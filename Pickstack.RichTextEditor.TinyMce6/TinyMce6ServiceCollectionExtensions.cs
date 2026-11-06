using Pickstack.RichTextEditor.TinyMce;

namespace Pickstack.RichTextEditor.TinyMce6;

/// <summary>
/// Provides extension methods for tiny mce6service collections.
/// </summary>
public static class TinyMce6ServiceCollectionExtensions
{
    /// <summary>
    /// The key constant.
    /// </summary>
    public const string Key = "TinyMCE6";

    /// <summary>
    /// The local script URL constant.
    /// </summary>
    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.TinyMce6/tinymce.min.js";

    /// <summary>
    /// Adds the tiny mce6.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>The tiny mce6.</returns>
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