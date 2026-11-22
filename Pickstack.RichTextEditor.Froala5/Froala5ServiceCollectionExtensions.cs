using Pickstack.RichTextEditor.Froala;

namespace Pickstack.RichTextEditor.Froala5;

/// <summary>Registers Froala 5.5.0.</summary>
public static class Froala5ServiceCollectionExtensions
{
    /// <summary>The configuration name. The value is <c>Froala5</c>.</summary>
    public const string Key = "Froala5";

    /// <summary>The script shipped in this package. The value is <c>/_content/Pickstack.RichTextEditor.Froala5/froala_editor.pkgd.min.js</c>.</summary>
    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.Froala5/froala_editor.pkgd.min.js";

    /// <summary>The stylesheet shipped in this package. The value is <c>/_content/Pickstack.RichTextEditor.Froala5/froala_editor.pkgd.min.css</c>.</summary>
    public const string LocalStyleUrl = "/_content/Pickstack.RichTextEditor.Froala5/froala_editor.pkgd.min.css";

    /// <summary>
    /// Registers Froala 5 as the only editor. The CDN script and stylesheet are loaded first, and the copies in this package are the fallbacks.
    /// <paramref name="configure"/> runs after those defaults and can replace them.
    /// </summary>
    /// <param name="builder">The Pickstack builder.</param>
    /// <param name="configure">Adjusts the options. <see langword="null"/> keeps the Froala 5 defaults.</param>
    /// <returns>The same builder.</returns>
    public static PickstackBuilder AddFroala5(this PickstackBuilder builder, Action<FroalaOptions>? configure = null)
    {
        return builder.AddFroala(options =>
        {
            options.Name = "Froala 5";
            options.ScriptUrl = "https://cdn.jsdelivr.net/npm/froala-editor@5.5.0/js/froala_editor.pkgd.min.js";
            options.FallbackUrl = LocalScriptUrl;
            options.StyleUrl = "https://cdn.jsdelivr.net/npm/froala-editor@5.5.0/css/froala_editor.pkgd.min.css";
            options.FallbackStyleUrl = LocalStyleUrl;
            configure?.Invoke(options);
        });
    }
}