using Pickstack.RichTextEditor.TinyMce;

namespace Pickstack.RichTextEditor.TinyMce7;

/// <summary>
/// Provides extension methods for tiny mce7service collections.
/// </summary>
public static class TinyMce7ServiceCollectionExtensions
{
    /// <summary>
    /// The key constant.
    /// </summary>
    public const string Key = "TinyMCE7";

    /// <summary>
    /// The local script URL constant.
    /// </summary>
    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.TinyMce7/tinymce.min.js";

    /// <summary>
    /// Adds the tiny mce7.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>The tiny mce7.</returns>
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