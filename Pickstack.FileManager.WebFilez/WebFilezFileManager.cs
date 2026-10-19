using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.FileManager.WebFilez;

public sealed class WebFilezFileManager : IFileManager, IPickstackAssetSource
{
    public const string Key = "WebFilez";

    private readonly WebFilezFileManagerOptions _options;

    public WebFilezFileManager(IOptions<WebFilezFileManagerOptions> options)
    {
        _options = options.Value;
    }

    public string Name => Key;

    public string ProviderScriptUrl => "/pickstack/assets/webfilez.js";

    public IReadOnlyList<ClientAsset> GetClientAssets() => [];

    public string GetProviderConfiguration() => JsonSerializer.Serialize(new
    {
        browserUrl = WebFilezRoutes.BrowserPath
    });

    public IEnumerable<PickstackAsset> GetAssets() => [
        PickstackAssets.Script(typeof(WebFilezFileManager).Assembly, "webfilez.js", "webfilez.js")
    ];
}
