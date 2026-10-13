using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pickstack.RichTextEditor.TinyMce;

public static class TinyMceServiceCollectionExtensions
{
    public static PickstackBuilder AddTinyMce(this PickstackBuilder builder, Action<TinyMceOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (configure is not null)
        {
            builder.Services.Configure(configure);
        }

        builder.Services.RemoveAll<IRichTextEditor>();
        builder.Services.AddSingleton<TinyMceEditor>();
        builder.Services.AddSingleton<IRichTextEditor>(services => services.GetRequiredService<TinyMceEditor>());
        builder.Services.AddSingleton<IPickstackAssetSource>(services => services.GetRequiredService<TinyMceEditor>());
        return builder;
    }
}