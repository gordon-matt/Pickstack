using Pickstack.RichTextEditor.Froala;

namespace Pickstack.RichTextEditor.Froala5;

/// <summary>
/// Provides extension methods for froala5service collections.
/// </summary>
public static class Froala5ServiceCollectionExtensions
{
    /// <summary>
    /// The key constant.
    /// </summary>
    public const string Key = "Froala5";

    /// <summary>
    /// The local script URL constant.
    /// </summary>
    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.Froala5/froala_editor.pkgd.min.js";

    /// <summary>
    /// The local style URL constant.
    /// </summary>
    public const string LocalStyleUrl = "/_content/Pickstack.RichTextEditor.Froala5/froala_editor.pkgd.min.css";

    /// <summary>
    /// Adds the froala5.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>The froala5.</returns>
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