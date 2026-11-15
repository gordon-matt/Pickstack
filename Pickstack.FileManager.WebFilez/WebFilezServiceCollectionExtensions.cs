using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using WebFilez;
using WebFilez.Services.Access;

namespace Pickstack.FileManager.WebFilez;

/// <summary>
/// Represents the web filez file manager startup.
/// </summary>
public sealed class WebFilezFileManagerStartup : IPickstackStartup
{
    /// <summary>
    /// Uses the value.
    /// </summary>
    /// <param name="app">The app.</param>
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

/// <summary>
/// Provides extension methods for web filez service collections.
/// </summary>
public static class WebFilezServiceCollectionExtensions
{
    /// <summary>
    /// Adds the web filez.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>A builder that accepts WebFilez package methods. Use <see cref="WebFilezPickstackBuilder.Pickstack"/> to continue with Pickstack.</returns>
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
