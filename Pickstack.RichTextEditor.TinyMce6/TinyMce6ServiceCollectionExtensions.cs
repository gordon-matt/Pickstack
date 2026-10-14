using Pickstack.RichTextEditor.TinyMce;

namespace Pickstack.RichTextEditor.TinyMce6;

public static class TinyMce6ServiceCollectionExtensions
{
    public const string Key = "TinyMCE6";

    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.TinyMce6/tinymce.min.js";

    public static PickstackBuilder AddTinyMce6(this PickstackBuilder builder, Action<TinyMceOptions>? configure = null)
    {
        return builder.AddTinyMce(options =>
        {
            options.Name = "TinyMCE 6";
            options.ScriptUrl = "https://cdn.jsdelivr.net/npm/tinymce@6.8.5/tinymce.min.js";
            options.FallbackUrl = LocalScriptUrl;
            options.IncludeLicense = false;
            configure?.Invoke(options);
        });
    }
}