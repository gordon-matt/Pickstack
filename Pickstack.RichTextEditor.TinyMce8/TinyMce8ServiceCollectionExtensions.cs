using Pickstack.RichTextEditor.TinyMce;

namespace Pickstack.RichTextEditor.TinyMce8;

/// <summary>
/// Provides extension methods for tiny mce8service collections.
/// </summary>
public static class TinyMce8ServiceCollectionExtensions
{
    /// <summary>
    /// The key constant.
    /// </summary>
    public const string Key = "TinyMCE8";

    /// <summary>
    /// The local script URL constant.
    /// </summary>
    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.TinyMce8/tinymce.min.js";

    /// <summary>
    /// Adds the tiny mce8.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>The tiny mce8.</returns>
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
