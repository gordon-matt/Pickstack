using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using WebFilez;
using WebFilez.Services.Access;

namespace Pickstack.FileManager.WebFilez;

public sealed class WebFilezFileManagerStartup : IPickstackStartup
{
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

public static class WebFilezServiceCollectionExtensions
{
    public static PickstackBuilder AddWebFilez(this PickstackBuilder builder, Action<WebFilezFileManagerOptions>? configure = null)
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
        builder.Services.AddWebFilez(configure: probe.ConfigureServer);
        builder.Services.AddWebFilezComponents();
        builder.Services.AddServerSideBlazor();

        builder.Services.RemoveAll<IFileManager>();
        builder.Services.AddSingleton<WebFilezFileManager>();
        builder.Services.AddSingleton<IFileManager>(services => services.GetRequiredService<WebFilezFileManager>());
        builder.Services.AddSingleton<IPickstackAssetSource>(services => services.GetRequiredService<WebFilezFileManager>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPickstackStartup, WebFilezFileManagerStartup>());
        return builder;
    }
}
