using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pickstack.FileManager.elFinder;

/// <summary>
/// Provides extension methods for el finder service collections.
/// </summary>
public static class ElFinderServiceCollectionExtensions
{
    /// <summary>
    /// Adds the el finder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>The el finder.</returns>
    public static PickstackBuilder AddElFinder(this PickstackBuilder builder, Action<ElFinderOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (configure is not null)
        {
            builder.Services.Configure(configure);
        }

        builder.Services.RemoveAll<IFileManager>();
        builder.Services.AddSingleton<ElFinderFileManager>();
        builder.Services.AddSingleton<IFileManager>(services => services.GetRequiredService<ElFinderFileManager>());
        builder.Services.AddSingleton<IPickstackAssetSource>(services => services.GetRequiredService<ElFinderFileManager>());
        return builder;
    }
}