using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Pickstack.FileManager.elFinder;

public sealed class ElFinderFileManager : IFileManager, IPickstackAssetSource
{
    public const string Key = "elFinder";

    private readonly ElFinderOptions _options;

    public ElFinderFileManager(IOptions<ElFinderOptions> options)
    {
        _options = options.Value;
    }

    public string Name => Key;

    public string ProviderScriptUrl => "/pickstack/assets/elfinder.js";

    public IReadOnlyList<ClientAsset> GetClientAssets() => [
        new ClientAsset { Url = _options.JQueryUiStyleUrl, Kind = ClientAssetKind.Style },
        new ClientAsset { Url = _options.StyleUrl, Kind = ClientAssetKind.Style },
        new ClientAsset { Url = _options.ThemeUrl, Kind = ClientAssetKind.Style },
        new ClientAsset { Url = _options.JQueryUiScriptUrl, Kind = ClientAssetKind.Script },
        new ClientAsset { Url = _options.ScriptUrl, Kind = ClientAssetKind.Script }
    ];

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

    public IEnumerable<PickstackAsset> GetAssets() => [
        PickstackAssets.Script(typeof(ElFinderFileManager).Assembly, "elfinder.js", "elfinder.js")
    ];
}