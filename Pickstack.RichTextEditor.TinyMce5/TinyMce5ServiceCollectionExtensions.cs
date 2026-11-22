using Pickstack.RichTextEditor.TinyMce;

namespace Pickstack.RichTextEditor.TinyMce5;

/// <summary>Registers TinyMCE 5.10.9.</summary>
public static class TinyMce5ServiceCollectionExtensions
{
    /// <summary>The configuration name. The value is <c>TinyMCE5</c>.</summary>
    public const string Key = "TinyMCE5";

    /// <summary>The copy shipped in this package. The value is <c>/_content/Pickstack.RichTextEditor.TinyMce5/tinymce.min.js</c>.</summary>
    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.TinyMce5/tinymce.min.js";

    /// <summary>
    /// Registers TinyMCE 5 as the only editor. The CDN copy is loaded first and <see cref="LocalScriptUrl"/> is the fallback.
    /// License and promotion settings are not sent. <paramref name="configure"/> runs after those defaults and can replace them.
    /// </summary>
    /// <param name="builder">The Pickstack builder.</param>
    /// <param name="configure">Adjusts the options. <see langword="null"/> keeps the TinyMCE 5 defaults.</param>
    /// <returns>The same builder.</returns>
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