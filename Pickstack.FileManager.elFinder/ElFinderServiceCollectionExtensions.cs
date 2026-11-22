using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Pickstack.FileManager.elFinder;

/// <summary>Registers elFinder as the file manager.</summary>
public static class ElFinderServiceCollectionExtensions
{
    /// <summary>
    /// Registers elFinder as the only <see cref="IFileManager"/>, serves <c>elfinder.js</c>, and starts the video-thumbnail search.
    /// A file manager already registered is replaced.
    /// </summary>
    /// <param name="builder">The Pickstack builder.</param>
    /// <param name="configure">Adjusts the client options. <see langword="null"/> keeps the defaults.</param>
    /// <returns>The same builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <see langword="null"/>.</exception>
    public static PickstackBuilder AddElFinder(this PickstackBuilder builder, Action<ElFinderOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (configure is not null)
        {
            builder.Services.Configure(configure);
        }

        builder.Services.AddHostedService<ElFinderVideoStartup>();
        builder.Services.RemoveAll<IFileManager>();
        builder.Services.AddSingleton<ElFinderFileManager>();
        builder.Services.AddSingleton<IFileManager>(services => services.GetRequiredService<ElFinderFileManager>());
        builder.Services.AddSingleton<IPickstackAssetSource>(services => services.GetRequiredService<ElFinderFileManager>());
        return builder;
    }
}