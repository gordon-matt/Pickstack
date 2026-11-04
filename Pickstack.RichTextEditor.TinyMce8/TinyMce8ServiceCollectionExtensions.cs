using Pickstack.RichTextEditor.TinyMce;

namespace Pickstack.RichTextEditor.TinyMce8;

public static class TinyMce8ServiceCollectionExtensions
{
    public const string Key = "TinyMCE8";

    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.TinyMce8/tinymce.min.js";

    public static PickstackBuilder AddTinyMce8(this PickstackBuilder builder, Action<TinyMceOptions>? configure = null)
    {
        return builder.AddTinyMce(options =>
        {
            options.Name = "TinyMCE 8";
            options.ScriptUrl = "https://cdn.jsdelivr.net/npm/tinymce@8.10.1/tinymce.min.js";
            options.FallbackUrl = LocalScriptUrl;
            options.IncludeLicense = true;
            configure?.Invoke(options);
        });
    }
}
