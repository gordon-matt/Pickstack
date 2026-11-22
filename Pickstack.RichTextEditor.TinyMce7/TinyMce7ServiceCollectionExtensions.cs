using Pickstack.RichTextEditor.TinyMce;

namespace Pickstack.RichTextEditor.TinyMce7;

/// <summary>Registers TinyMCE 7.9.0.</summary>
public static class TinyMce7ServiceCollectionExtensions
{
    /// <summary>The configuration name. The value is <c>TinyMCE7</c>.</summary>
    public const string Key = "TinyMCE7";

    /// <summary>The copy shipped in this package. The value is <c>/_content/Pickstack.RichTextEditor.TinyMce7/tinymce.min.js</c>.</summary>
    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.TinyMce7/tinymce.min.js";

    /// <summary>
    /// Registers TinyMCE 7 as the only editor. The CDN copy is loaded first and <see cref="LocalScriptUrl"/> is the fallback.
    /// The GPL license key is sent. <paramref name="configure"/> runs after those defaults and can replace them.
    /// </summary>
    /// <param name="builder">The Pickstack builder.</param>
    /// <param name="configure">Adjusts the options. <see langword="null"/> keeps the TinyMCE 7 defaults.</param>
    /// <returns>The same builder.</returns>
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