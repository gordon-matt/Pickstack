using Pickstack;
using Pickstack.FileManager.elFinder;
using Pickstack.FileManager.ResponsiveFileManager;
using Pickstack.RichTextEditor.CkEditor;
using Pickstack.RichTextEditor.TinyMce;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var stack = builder.Configuration.GetSection("Pickstack");
var pickstack = builder.Services.AddPickstack(builder.Configuration);

pickstack = stack["FileManager"] switch
{
    ResponsiveFileManagerAdapter.Key => pickstack.AddResponsiveFileManager(),
    null or "" or ElFinderFileManager.Key => pickstack.AddElFinder(),
    var name => throw new InvalidOperationException(
        $"Unknown Pickstack file manager '{name}'. Use '{ElFinderFileManager.Key}' or '{ResponsiveFileManagerAdapter.Key}'.")
};

pickstack = stack["RichTextEditor"] switch
{
    CkEditorEditor.Key => pickstack.AddCkEditor(),
    null or "" or TinyMceEditor.Key => pickstack.AddTinyMce(),
    var name => throw new InvalidOperationException(
        $"Unknown Pickstack rich text editor '{name}'. Use '{TinyMceEditor.Key}' or '{CkEditorEditor.Key}'.")
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