using Pickstack;
using Pickstack.FileManager.elFinder;
using Pickstack.FileManager.ResponsiveFileManager;
using Pickstack.RichTextEditor.CkEditor;
using Pickstack.RichTextEditor.TinyMce;
using Pickstack.RichTextEditor.TinyMce5;
using Pickstack.RichTextEditor.TinyMce6;
using Pickstack.RichTextEditor.TinyMce7;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var stack = builder.Configuration.GetSection("Pickstack");
var pickstack = builder.Services.AddPickstack(builder.Configuration);

pickstack = stack["FileManager"] switch
{
    ResponsiveFileManagerAdapter.Key => pickstack.AddResponsiveFileManager(fileManager => fileManager
        .WithLanguage("en_EN")
        .Server(server => server.MaxSizeUpload = 16)),
    null or "" or ElFinderFileManager.Key => pickstack.AddElFinder(fileManager => fileManager
        .DisableCommands("extract", "archive")),
    var name => throw new InvalidOperationException(
        $"Unknown Pickstack file manager '{name}'. Use '{ElFinderFileManager.Key}' or '{ResponsiveFileManagerAdapter.Key}'.")
};

Action<TinyMceOptions> tinymce = editor => editor
    .AddPlugins("codesample", "charmap")
    .WithToolbar("undo redo | bold italic underline | alignleft aligncenter alignright | bullist numlist | link image media charmap codesample | code");

pickstack = stack["RichTextEditor"] switch
{
    CkEditorEditor.Key => pickstack.AddCkEditor(editor => editor
        .WithoutPlugins("elementspath")),
    TinyMce5ServiceCollectionExtensions.Key => pickstack.AddTinyMce5(tinymce),
    TinyMce6ServiceCollectionExtensions.Key => pickstack.AddTinyMce6(tinymce),
    null or "" or TinyMceEditor.Key or TinyMce7ServiceCollectionExtensions.Key => pickstack.AddTinyMce7(tinymce),
    var name => throw new InvalidOperationException(
        $"Unknown Pickstack rich text editor '{name}'. Use '{TinyMce5ServiceCollectionExtensions.Key}', '{TinyMce6ServiceCollectionExtensions.Key}', '{TinyMce7ServiceCollectionExtensions.Key}', or '{CkEditorEditor.Key}'.")
};

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UsePickstack();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();