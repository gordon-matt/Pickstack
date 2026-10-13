using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Pickstack.FileManager.ResponsiveFileManager;

public sealed class ResponsiveFileManagerStartup : IPickstackStartup
{
    public void Use(WebApplication app)
    {
        app.UseSession();
        app.UseResponsiveFileManager();
    }
}

public static class ResponsiveFileManagerServiceCollectionExtensions
{
    public static PickstackBuilder AddResponsiveFileManager(
        this PickstackBuilder builder,
        Action<global::ResponsiveFileManager.ResponsiveFileManagerOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // The package applies a single IConfigureOptions instance. Register the path
        // mapping last so it is the instance their middleware resolves.
        builder.Services.AddResponsiveFileManager(_ => { });
        builder.Services.AddSingleton<IConfigureOptions<global::ResponsiveFileManager.ResponsiveFileManagerOptions>>(services =>
            new ResponsiveFileManagerConfigurator(services.GetRequiredService<IFileStorage>(), configure));

        builder.Services.RemoveAll<IFileManager>();
        builder.Services.AddSingleton<ResponsiveFileManagerAdapter>();
        builder.Services.AddSingleton<IFileManager>(services => services.GetRequiredService<ResponsiveFileManagerAdapter>());
        builder.Services.AddSingleton<IPickstackAssetSource>(services => services.GetRequiredService<ResponsiveFileManagerAdapter>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPickstackStartup, ResponsiveFileManagerStartup>());
        return builder;
    }
}