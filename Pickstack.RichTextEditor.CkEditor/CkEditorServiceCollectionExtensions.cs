using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pickstack.RichTextEditor.CkEditor;

public static class CkEditorServiceCollectionExtensions
{
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