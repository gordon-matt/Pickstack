using Pickstack.RichTextEditor.TinyMce;

namespace Pickstack.RichTextEditor.TinyMce5;

public static class TinyMce5ServiceCollectionExtensions
{
    public const string Key = "TinyMCE5";

    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.TinyMce5/tinymce.min.js";

    public static PickstackBuilder AddTinyMce5(this PickstackBuilder builder, Action<TinyMceOptions>? configure = null)
    {
        return builder.AddTinyMce(options =>
        {
            options.Name = "TinyMCE 5";
            options.ScriptUrl = "https://cdn.jsdelivr.net/npm/tinymce@5.10.9/tinymce.min.js";
            options.FallbackUrl = LocalScriptUrl;
            options.IncludeLicense = false;
            configure?.Invoke(options);
        });
    }
}