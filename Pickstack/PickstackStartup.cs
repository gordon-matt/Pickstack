using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pickstack.FileManager;

namespace Pickstack;

public sealed class PickstackRuntimeAssets : IPickstackAssetSource
{
    public IEnumerable<PickstackAsset> GetAssets() => [
        PickstackAssets.Script(typeof(PickstackRuntimeAssets).Assembly, "pickstack.js", "pickstack.js"),
        PickstackAssets.Script(typeof(PickstackRuntimeAssets).Assembly, "load.js", "load.js")
    ];
}

public sealed class PickstackStartup : IPickstackStartup
{
    public void Use(WebApplication app)
    {
        app.Services.GetRequiredService<IFileStorage>().EnsureCreated();

        app.MapGet("/pickstack/files/root", (string? root, IFileStorage storage) =>
        {
            try
            {
                storage.EnsureRoot(root);
                return Results.NoContent();
            }
            catch (InvalidOperationException)
            {
                return Results.BadRequest();
            }
        });

        app.MapGet("/pickstack/assets/{name}", async (HttpContext context, string name, IEnumerable<IPickstackAssetSource> sources) =>
        {
            var asset = sources
                .SelectMany(source => source.GetAssets())
                .FirstOrDefault(candidate => string.Equals(candidate.Name, name, StringComparison.OrdinalIgnoreCase));

            if (asset is null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            context.Response.ContentType = asset.ContentType;
            context.Response.Headers.CacheControl = "no-cache";
            await using var stream = asset.OpenRead();
            await stream.CopyToAsync(context.Response.Body);
        });
    }
}

public static class WebApplicationExtensions
{
    public static WebApplication UsePickstack(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        foreach (var startup in app.Services.GetServices<IPickstackStartup>())
        {
            startup.Use(app);
        }

        return app;
    }
}