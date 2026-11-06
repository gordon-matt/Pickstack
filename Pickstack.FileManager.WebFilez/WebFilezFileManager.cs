using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.FileManager.WebFilez;

/// <summary>
/// Represents the web filez file manager.
/// </summary>
public sealed class WebFilezFileManager : IFileManager, IPickstackAssetSource
{
    /// <summary>
    /// The key constant.
    /// </summary>
    public const string Key = "WebFilez";

    private readonly WebFilezFileManagerOptions _options;

    /// <summary>
    /// Initializes a new instance of the WebFilezFileManager class.
    /// </summary>
    /// <param name="options">The options.</param>
    public WebFilezFileManager(IOptions<WebFilezFileManagerOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name => Key;

    /// <summary>
    /// Gets the provider script URL.
    /// </summary>
    public string ProviderScriptUrl => "/pickstack/assets/webfilez.js";

    /// <summary>
    /// Gets the client assets.
    /// </summary>
    /// <returns>The client assets.</returns>
    public IReadOnlyList<ClientAsset> GetClientAssets() => [];

    /// <summary>
    /// Gets the provider configuration.
    /// </summary>
    /// <returns>The provider configuration.</returns>
    public string GetProviderConfiguration() => JsonSerializer.Serialize(new
    {
        browserUrl = WebFilezRoutes.BrowserPath
    });

    /// <summary>
    /// Gets the assets.
    /// </summary>
    /// <returns>The assets.</returns>
    public IEnumerable<PickstackAsset> GetAssets() => [
        PickstackAssets.Script(typeof(WebFilezFileManager).Assembly, "webfilez.js", "webfilez.js")
    ];
}
