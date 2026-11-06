using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pickstack.RichTextEditor.CkEditor;

/// <summary>
/// Provides extension methods for ck editor service collections.
/// </summary>
public static class CkEditorServiceCollectionExtensions
{
    /// <summary>
    /// Adds the ck editor.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>The ck editor.</returns>
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