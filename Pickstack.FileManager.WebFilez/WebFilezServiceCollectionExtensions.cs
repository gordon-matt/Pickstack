using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using WebFilez;
using WebFilez.Services.Access;

namespace Pickstack.FileManager.WebFilez;

/// <summary>Maps the Blazor circuit and the WebFilez API unless the host already mapped them.</summary>
public sealed class WebFilezFileManagerStartup : IPickstackStartup
{
    /// <summary>
    /// Calls <c>MapBlazorHub</c> unless <see cref="WebFilezFileManagerOptions.UseHostBlazor"/> was used,
    /// and <c>MapWebFilezApi</c> unless <see cref="WebFilezFileManagerOptions.UseHostApi"/> was used.
    /// </summary>
    /// <param name="app">The application being started.</param>
    public void Use(WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<WebFilezFileManagerOptions>>().Value;
        if (!options.HostOwnsBlazor)
        {
            app.MapBlazorHub();
        }

        if (!options.HostOwnsApi)
        {
            app.MapWebFilezApi();
        }
    }
}

/// <summary>Registers WebFilez as the file manager.</summary>
public static class WebFilezServiceCollectionExtensions
{
    /// <summary>
    /// Registers WebFilez as the only <see cref="IFileManager"/>, the Blazor Server circuit, and the WebFilez components.
    /// The shared upload folder is published as the <c>media</c> root. A file manager already registered is replaced.
    /// </summary>
    /// <param name="builder">The Pickstack builder.</param>
    /// <param name="configure">
    /// Adjusts the browser options. It runs once during registration so the host-owned circuit and API decisions can be read, and again when the options are created.
    /// <see langword="null"/> keeps the defaults.
    /// </param>
    /// <returns>A builder that accepts WebFilez package methods. Use <see cref="WebFilezPickstackBuilder.Pickstack"/> to continue with Pickstack.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <see langword="null"/>.</exception>
    public static WebFilezPickstackBuilder AddWebFilez(this PickstackBuilder builder, Action<WebFilezFileManagerOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var probe = new WebFilezFileManagerOptions();
        configure?.Invoke(probe);
        if (configure is not null)
        {
            builder.Services.Configure(configure);
        }

        // Registered first so AddWebFilez's TryAdd leaves this provider in place.
        builder.Services.AddScoped<IFileRootProvider, PickstackFileRootProvider>();
        builder.Services.AddWebFilez(configure: probe.ConfigureServer).AddComponents();
        builder.Services.AddServerSideBlazor();

        builder.Services.RemoveAll<IFileManager>();
        builder.Services.AddSingleton<WebFilezFileManager>();
        builder.Services.AddSingleton<IFileManager>(services => services.GetRequiredService<WebFilezFileManager>());
        builder.Services.AddSingleton<IPickstackAssetSource>(services => services.GetRequiredService<WebFilezFileManager>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPickstackStartup, WebFilezFileManagerStartup>());
        return new WebFilezPickstackBuilder(builder);
    }
}
