using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.FileManager.WebFilez;

/// <summary>WebFilez as <see cref="IFileManager"/>. The browser runs in a frame, so the host page does not load Blazor.</summary>
public sealed class WebFilezFileManager : IFileManager, IPickstackAssetSource
{
    /// <summary>The configuration name and the value of <see cref="Name"/>. The value is <c>WebFilez</c>.</summary>
    public const string Key = "WebFilez";

    private readonly WebFilezFileManagerOptions _options;

    /// <summary>Reads the browser options.</summary>
    /// <param name="options">The WebFilez options.</param>
    public WebFilezFileManager(IOptions<WebFilezFileManagerOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>The display name. The value is <see cref="Key"/>.</summary>
    public string Name => Key;

    /// <summary>The embedded provider script. The value is <c>/pickstack/assets/webfilez.js</c>.</summary>
    public string ProviderScriptUrl => "/pickstack/assets/webfilez.js";

    /// <summary>No extra styles or scripts. The frame page loads the browser.</summary>
    /// <returns>An empty list.</returns>
    public IReadOnlyList<ClientAsset> GetClientAssets() => [];

    /// <summary>JSON for the provider script. It contains the browser page address.</summary>
    /// <returns>The JSON.</returns>
    public string GetProviderConfiguration() => JsonSerializer.Serialize(new
    {
        browserUrl = WebFilezRoutes.BrowserPath
    });

    /// <summary>The embedded <c>webfilez.js</c> provider script.</summary>
    /// <returns>The asset.</returns>
    public IEnumerable<PickstackAsset> GetAssets() => [
        PickstackAssets.Script(typeof(WebFilezFileManager).Assembly, "webfilez.js", "webfilez.js")
    ];
}
