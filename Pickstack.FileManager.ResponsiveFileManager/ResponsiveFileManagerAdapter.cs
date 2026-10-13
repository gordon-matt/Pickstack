using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Pickstack.FileManager.ResponsiveFileManager;

public sealed class ResponsiveFileManagerOptions
{
    public string DialogUrl { get; set; } = "/filemanager/dialog.php";
}

public sealed class ResponsiveFileManagerAdapter : IFileManager, IPickstackAssetSource
{
    public const string Key = "ResponsiveFileManager";

    private readonly ResponsiveFileManagerOptions _options;

    public ResponsiveFileManagerAdapter(IOptions<ResponsiveFileManagerOptions> options)
    {
        _options = options.Value;
    }

    public string Name => Key;

    public string ProviderScriptUrl => "/pickstack/assets/responsive-file-manager.js";

    public IReadOnlyList<ClientAsset> GetClientAssets() => [];

    public string GetProviderConfiguration() => JsonSerializer.Serialize(new
    {
        dialogUrl = _options.DialogUrl
    });

    public IEnumerable<PickstackAsset> GetAssets() => [
        PickstackAssets.Script(typeof(ResponsiveFileManagerAdapter).Assembly, "responsive-file-manager.js", "responsive-file-manager.js")
    ];
}