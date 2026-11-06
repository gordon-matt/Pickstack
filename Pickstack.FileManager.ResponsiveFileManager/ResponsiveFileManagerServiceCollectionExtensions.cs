using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Pickstack.FileManager.ResponsiveFileManager;

/// <summary>
/// Represents the responsive file manager startup.
/// </summary>
public sealed class ResponsiveFileManagerStartup : IPickstackStartup
{
    /// <summary>
    /// Uses the value.
    /// </summary>
    /// <param name="app">The app.</param>
    public void Use(WebApplication app)
    {
        if (!app.Services.GetRequiredService<IOptions<ResponsiveFileManagerOptions>>().Value.HostOwnsSession)
        {
            app.UseSession();
        }

        app.UseResponsiveFileManager();
    }
}

/// <summary>
/// Provides extension methods for responsive file manager service collections.
/// </summary>
public static class ResponsiveFileManagerServiceCollectionExtensions
{
    /// <summary>
    /// Adds the responsive file manager.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>The responsive file manager.</returns>
    public static PickstackBuilder AddResponsiveFileManager(
        this PickstackBuilder builder,
        Action<ResponsiveFileManagerOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var probe = new ResponsiveFileManagerOptions();
        configure?.Invoke(probe);
        if (configure is not null)
        {
            builder.Services.Configure(configure);
        }

        if (!probe.HostOwnsSession)
        {
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        // The package applies a single IConfigureOptions instance. Register the path
        // mapping last so it is the instance their middleware resolves.
        builder.Services.AddResponsiveFileManager(_ => { });
        builder.Services.AddSingleton<IConfigureOptions<global::ResponsiveFileManager.ResponsiveFileManagerOptions>>(services =>
            new ResponsiveFileManagerConfigurator(
                services.GetRequiredService<IFileStorage>(),
                services.GetRequiredService<IOptions<ResponsiveFileManagerOptions>>()));

        builder.Services.RemoveAll<IFileManager>();
        builder.Services.AddSingleton<ResponsiveFileManagerAdapter>();
        builder.Services.AddSingleton<IFileManager>(services => services.GetRequiredService<ResponsiveFileManagerAdapter>());
        builder.Services.AddSingleton<IPickstackAssetSource>(services => services.GetRequiredService<ResponsiveFileManagerAdapter>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPickstackStartup, ResponsiveFileManagerStartup>());
        return builder;
    }
}