using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pickstack.FileManager.elFinder;

public static class ElFinderServiceCollectionExtensions
{
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