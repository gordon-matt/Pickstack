using Pickstack.RichTextEditor.TinyMce;

namespace Pickstack.RichTextEditor.TinyMce5;

/// <summary>
/// Provides extension methods for tiny mce5service collections.
/// </summary>
public static class TinyMce5ServiceCollectionExtensions
{
    /// <summary>
    /// The key constant.
    /// </summary>
    public const string Key = "TinyMCE5";

    /// <summary>
    /// The local script URL constant.
    /// </summary>
    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.TinyMce5/tinymce.min.js";

    /// <summary>
    /// Adds the tiny mce5.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>The tiny mce5.</returns>
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