using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pickstack.RichTextEditor.CkEditor;

/// <summary>Registers CKEditor 4 as the rich text editor.</summary>
public static class CkEditorServiceCollectionExtensions
{
    /// <summary>
    /// Registers CKEditor as the only <see cref="IRichTextEditor"/> and serves <c>ckeditor.js</c>.
    /// An editor already registered is replaced. With no file manager, CKEditor keeps its own dialogs.
    /// </summary>
    /// <param name="builder">The Pickstack builder.</param>
    /// <param name="configure">Adjusts the options. <see langword="null"/> keeps the defaults, including an empty toolbar.</param>
    /// <returns>The same builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <see langword="null"/>.</exception>
    public static PickstackBuilder AddCkEditor(this PickstackBuilder builder, Action<CkEditorOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (configure is not null)
        {
            builder.Services.Configure(configure);
        }

        builder.Services.RemoveAll<IRichTextEditor>();
        builder.Services.AddSingleton<CkEditorEditor>();
        builder.Services.AddSingleton<IRichTextEditor>(services => services.GetRequiredService<CkEditorEditor>());
        builder.Services.AddSingleton<IPickstackAssetSource>(services => services.GetRequiredService<CkEditorEditor>());
        return builder;
    }
}