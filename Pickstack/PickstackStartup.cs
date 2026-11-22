using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pickstack.FileManager;

namespace Pickstack;

/// <summary>The embedded browser scripts: <c>pickstack.js</c>, <c>load.js</c>, and <c>editor.js</c>.</summary>
public sealed class PickstackRuntimeAssets : IPickstackAssetSource
{
    /// <summary>The three embedded scripts, served from <c>/pickstack/assets/{name}</c>.</summary>
    /// <returns>The scripts. Opening one throws when the embedded resource is missing.</returns>
    public IEnumerable<PickstackAsset> GetAssets() => [
        PickstackAssets.Script(typeof(PickstackRuntimeAssets).Assembly, "pickstack.js", "pickstack.js"),
        PickstackAssets.Script(typeof(PickstackRuntimeAssets).Assembly, "load.js", "load.js"),
        PickstackAssets.Script(typeof(PickstackRuntimeAssets).Assembly, "editor.js", "editor.js")
    ];
}

/// <summary>Creates the upload folders and serves the shared browser scripts.</summary>
public sealed class PickstackStartup : IPickstackStartup
{
    /// <summary>
    /// Creates the upload and thumbnail folders, maps <c>GET /pickstack/files/root</c>, and maps <c>GET /pickstack/assets/{name}</c>.
    /// The root endpoint creates the requested folder and answers 204. An unsafe root answers 400.
    /// An unknown asset name answers 404. A served asset uses its content type and <c>Cache-Control: no-cache</c>.
    /// </summary>
    /// <param name="app">The application being started.</param>
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

/// <summary>Runs every registered <see cref="IPickstackStartup"/>.</summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Creates the upload folders, serves the shared scripts, and runs each adapter startup in registration order.
    /// </summary>
    /// <param name="app">The application being started.</param>
    /// <returns>The same application.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="app"/> is <see langword="null"/>.</exception>
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