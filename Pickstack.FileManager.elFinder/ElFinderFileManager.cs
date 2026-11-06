using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.FileManager.elFinder;

/// <summary>
/// Represents the el finder file manager.
/// </summary>
public sealed class ElFinderFileManager : IFileManager, IPickstackAssetSource
{
    /// <summary>
    /// The key constant.
    /// </summary>
    public const string Key = "elFinder";

    private readonly ElFinderOptions _options;

    /// <summary>
    /// Initializes a new instance of the ElFinderFileManager class.
    /// </summary>
    /// <param name="options">The options.</param>
    public ElFinderFileManager(IOptions<ElFinderOptions> options)
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
    public string ProviderScriptUrl => "/pickstack/assets/elfinder.js";

    /// <summary>
    /// Gets the client assets.
    /// </summary>
    /// <returns>The client assets.</returns>
    public IReadOnlyList<ClientAsset> GetClientAssets() => [
        new ClientAsset { Url = _options.JQueryUiStyleUrl, Kind = ClientAssetKind.Style },
        new ClientAsset { Url = _options.StyleUrl, Kind = ClientAssetKind.Style },
        new ClientAsset { Url = _options.ThemeUrl, Kind = ClientAssetKind.Style },
        new ClientAsset { Url = _options.JQueryUiScriptUrl, Kind = ClientAssetKind.Script },
        new ClientAsset { Url = _options.ScriptUrl, Kind = ClientAssetKind.Script }
    ];

    /// <summary>
    /// Gets the provider configuration.
    /// </summary>
    /// <returns>The provider configuration.</returns>
    public string GetProviderConfiguration()
    {
        string baseUrl = _options.BaseUrl.EndsWith('/') ? _options.BaseUrl : _options.BaseUrl + "/";
        return JsonSerializer.Serialize(new
        {
            connectorUrl = ElFinderRoutes.ConnectorPath,
            baseUrl,
            toolbar = _options.Toolbar,
            disabledCommands = _options.DisabledCommands,
            minimumHeight = _options.MinimumHeight,
            rememberLastDir = _options.RememberLastDir,
            resizable = _options.Resizable,
            useBrowserHistory = _options.UseBrowserHistory
        });
    }

    /// <summary>
    /// Gets the assets.
    /// </summary>
    /// <returns>The assets.</returns>
    public IEnumerable<PickstackAsset> GetAssets() => [
        PickstackAssets.Script(typeof(ElFinderFileManager).Assembly, "elfinder.js", "elfinder.js")
    ];
}