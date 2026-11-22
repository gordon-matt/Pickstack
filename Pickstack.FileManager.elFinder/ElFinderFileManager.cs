using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.FileManager.elFinder;

/// <summary>elFinder 2 as <see cref="IFileManager"/>. The connector URL and the upload folder stay owned by Pickstack.</summary>
public sealed class ElFinderFileManager : IFileManager, IPickstackAssetSource
{
    /// <summary>The configuration name and the value of <see cref="Name"/>. The value is <c>elFinder</c>.</summary>
    public const string Key = "elFinder";

    private readonly ElFinderOptions _options;

    /// <summary>Reads the client options.</summary>
    /// <param name="options">The elFinder options.</param>
    public ElFinderFileManager(IOptions<ElFinderOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>The display name. The value is <see cref="Key"/>.</summary>
    public string Name => Key;

    /// <summary>The embedded provider script. The value is <c>/pickstack/assets/elfinder.js</c>.</summary>
    public string ProviderScriptUrl => "/pickstack/assets/elfinder.js";

    /// <summary>jQuery UI, then the elFinder stylesheet, theme, jQuery UI script, and elFinder script, in that order.</summary>
    /// <returns>The assets. The page must already have loaded jQuery.</returns>
    public IReadOnlyList<ClientAsset> GetClientAssets() => [
        new ClientAsset { Url = _options.JQueryUiStyleUrl, Kind = ClientAssetKind.Style },
        new ClientAsset { Url = _options.StyleUrl, Kind = ClientAssetKind.Style },
        new ClientAsset { Url = _options.ThemeUrl, Kind = ClientAssetKind.Style },
        new ClientAsset { Url = _options.JQueryUiScriptUrl, Kind = ClientAssetKind.Script },
        new ClientAsset { Url = _options.ScriptUrl, Kind = ClientAssetKind.Script }
    ];

    /// <summary>
    /// JSON for the provider script: the connector address, client directory, toolbar, disabled commands, and the dialog flags.
    /// A <see cref="ElFinderOptions.BaseUrl"/> without a trailing slash is given one.
    /// </summary>
    /// <returns>The JSON.</returns>
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

    /// <summary>The embedded <c>elfinder.js</c> provider script.</summary>
    /// <returns>The asset.</returns>
    public IEnumerable<PickstackAsset> GetAssets() => [
        PickstackAssets.Script(typeof(ElFinderFileManager).Assembly, "elfinder.js", "elfinder.js")
    ];
}