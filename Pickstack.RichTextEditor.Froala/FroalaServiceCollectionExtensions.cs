using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pickstack.RichTextEditor.Froala;

/// <summary>
/// Provides extension methods for froala service collections.
/// </summary>
public static class FroalaServiceCollectionExtensions
{
    /// <summary>
    /// Adds the froala.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>The froala.</returns>
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