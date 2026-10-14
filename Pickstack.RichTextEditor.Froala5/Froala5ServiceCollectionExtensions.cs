using Pickstack.RichTextEditor.Froala;

namespace Pickstack.RichTextEditor.Froala5;

public static class Froala5ServiceCollectionExtensions
{
    public const string Key = "Froala5";

    public const string LocalScriptUrl = "/_content/Pickstack.RichTextEditor.Froala5/froala_editor.pkgd.min.js";

    public const string LocalStyleUrl = "/_content/Pickstack.RichTextEditor.Froala5/froala_editor.pkgd.min.css";

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