using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Pickstack.FileManager.ResponsiveFileManager;

/// <summary>Enables the Responsive File Manager middleware, and session middleware unless the host already owns the session.</summary>
public sealed class ResponsiveFileManagerStartup : IPickstackStartup
{
    /// <summary>
    /// Calls <c>UseSession</c> unless <see cref="ResponsiveFileManagerOptions.UseHostSession"/> was used, then enables the file manager middleware.
    /// </summary>
    /// <param name="app">The application being started.</param>
    public void Use(WebApplication app)
    {
        if (!app.Services.GetRequiredService<IOptions<ResponsiveFileManagerOptions>>().Value.HostOwnsSession)
        {
            app.UseSession();
        }

        app.UseResponsiveFileManager();
    }
}

/// <summary>Registers Responsive File Manager as the file manager.</summary>
public static class ResponsiveFileManagerServiceCollectionExtensions
{
    /// <summary>
    /// Registers Responsive File Manager as the only <see cref="IFileManager"/> and points its upload folder at the shared storage.
    /// A file manager already registered is replaced.
    /// Unless <see cref="ResponsiveFileManagerOptions.UseHostSession"/> is called, this also registers a memory cache and a 30-minute essential session cookie.
    /// </summary>
    /// <param name="builder">The Pickstack builder.</param>
    /// <param name="configure">
    /// Adjusts the dialog options. It runs once during registration so the session decision can be read, and again when the options are created.
    /// <see langword="null"/> keeps the defaults.
    /// </param>
    /// <returns>The same builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The configurator registered here is the one the middleware resolves. It sets the upload and thumbnail paths, then runs the <see cref="ResponsiveFileManagerOptions.Server"/> callback, which can replace those paths.
    /// </remarks>
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