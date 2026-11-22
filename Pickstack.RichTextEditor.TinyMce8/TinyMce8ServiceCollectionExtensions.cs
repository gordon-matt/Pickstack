using Pickstack.RichTextEditor.TinyMce;

namespace Pickstack.RichTextEditor.TinyMce8;

/// <summary>Registers TinyMCE 8.10.1.</summary>
public static class TinyMce8ServiceCollectionExtensions
{
    /// <summary>The configuration name. The value is <c>TinyMCE8</c>.</summary>
    public const string Key = "TinyMCE8";

    /// <summary>The copy shipped in this package. Skins and plugins are served from the same content root. The value is <c>/_content/Pickstack.RichTextEditor.TinyMce8/tinymce.min.js</c>.</summary>
    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.TinyMce8/tinymce.min.js";

    /// <summary>
    /// Registers TinyMCE 8 as the only editor. The CDN copy is loaded first and <see cref="LocalScriptUrl"/> is the fallback.
    /// The GPL license key is sent. <paramref name="configure"/> runs after those defaults and can replace them.
    /// </summary>
    /// <param name="builder">The Pickstack builder.</param>
    /// <param name="configure">Adjusts the options. <see langword="null"/> keeps the TinyMCE 8 defaults.</param>
    /// <returns>The same builder.</returns>
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
