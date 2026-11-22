using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pickstack.RichTextEditor.Froala;

/// <summary>Registers the shared Froala adapter.</summary>
public static class FroalaServiceCollectionExtensions
{
    /// <summary>
    /// Registers Froala as the only <see cref="IRichTextEditor"/> and serves <c>froala.js</c>.
    /// An editor already registered is replaced. <c>AddFroala5</c> calls this after setting the Froala 5 addresses.
    /// </summary>
    /// <param name="builder">The Pickstack builder.</param>
    /// <param name="configure">Adjusts the options. <see langword="null"/> keeps the defaults, including the Froala 5 CDN addresses.</param>
    /// <returns>The same builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <see langword="null"/>.</exception>
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