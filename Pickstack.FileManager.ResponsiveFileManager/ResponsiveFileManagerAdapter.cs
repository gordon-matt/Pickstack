using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Pickstack.FileManager.ResponsiveFileManager;

/// <summary>
/// Client options for the Responsive File Manager dialog.
/// Use <see cref="Server"/> for the underlying PHP configuration.
/// </summary>
public sealed class ResponsiveFileManagerOptions
{
    public string DialogUrl { get; set; } = "/filemanager/dialog.php";

    public bool RelativeUrls { get; set; }

    public bool CrossDomain { get; set; }

    public string? Language { get; set; }

    internal Action<global::ResponsiveFileManager.ResponsiveFileManagerOptions>? ConfigureServer { get; private set; }

    public ResponsiveFileManagerOptions WithDialogUrl(string url)
    {
        DialogUrl = url;
        return this;
    }

    public ResponsiveFileManagerOptions UseRelativeUrls(bool relative = true)
    {
        RelativeUrls = relative;
        return this;
    }

    public ResponsiveFileManagerOptions UseCrossDomain(bool enabled = true)
    {
        CrossDomain = enabled;
        return this;
    }

    public ResponsiveFileManagerOptions WithLanguage(string language)
    {
        Language = language;
        return this;
    }

    public ResponsiveFileManagerOptions Server(Action<global::ResponsiveFileManager.ResponsiveFileManagerOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        ConfigureServer = configure;
        return this;
    }
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
        dialogUrl = _options.DialogUrl,
        relativeUrls = _options.RelativeUrls,
        crossDomain = _options.CrossDomain,
        language = _options.Language
    });

    public IEnumerable<PickstackAsset> GetAssets() => [
        PickstackAssets.Script(typeof(ResponsiveFileManagerAdapter).Assembly, "responsive-file-manager.js", "responsive-file-manager.js")
    ];
}