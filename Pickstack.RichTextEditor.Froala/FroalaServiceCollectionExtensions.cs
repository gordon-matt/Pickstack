using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pickstack.RichTextEditor.Froala;

public static class FroalaServiceCollectionExtensions
{
    public static PickstackBuilder AddFroala(this PickstackBuilder builder, Action<FroalaOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (configure is not null)
        {
            builder.Services.Configure(configure);
        }

        builder.Services.RemoveAll<IRichTextEditor>();
        builder.Services.AddSingleton<FroalaEditor>();
        builder.Services.AddSingleton<IRichTextEditor>(services => services.GetRequiredService<FroalaEditor>());
        builder.Services.AddSingleton<IPickstackAssetSource>(services => services.GetRequiredService<FroalaEditor>());
        return builder;
    }
}