using Pickstack.RichTextEditor.TinyMce;

namespace Pickstack.RichTextEditor.TinyMce7;

public static class TinyMce7ServiceCollectionExtensions
{
    public const string Key = "TinyMCE7";

    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.TinyMce7/tinymce.min.js";

    public static PickstackBuilder AddTinyMce7(this PickstackBuilder builder, Action<TinyMceOptions>? configure = null)
    {
        return builder.AddTinyMce(options =>
        {
            options.Name = "TinyMCE 7";
            options.ScriptUrl = "https://cdn.jsdelivr.net/npm/tinymce@7.9.0/tinymce.min.js";
            options.FallbackUrl = LocalScriptUrl;
            options.IncludeLicense = true;
            configure?.Invoke(options);
        });
    }
}