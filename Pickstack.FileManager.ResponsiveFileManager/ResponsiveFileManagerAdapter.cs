using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.FileManager.ResponsiveFileManager;

/// <summary>
/// Client options for the Responsive File Manager dialog.
/// Use <see cref="Server"/> for the underlying PHP configuration.
/// </summary>
public sealed class ResponsiveFileManagerOptions
{
    /// <summary>
    /// Gets or sets the dialog URL.
    /// </summary>
    public string DialogUrl { get; set; } = "/filemanager/dialog.php";

    /// <summary>
    /// Gets or sets a value indicating whether this instance is relative urls.
    /// </summary>
    public bool RelativeUrls { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is cross domain.
    /// </summary>
    public bool CrossDomain { get; set; }

    /// <summary>
    /// Gets or sets the language.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// The host already calls <c>UseSession</c>. Pickstack then only enables the file manager middleware.
    /// </summary>
    public bool HostOwnsSession { get; private set; }

    internal Action<global::ResponsiveFileManager.ResponsiveFileManagerOptions>? ConfigureServer { get; private set; }

    /// <summary>
    /// Returns a copy with the dialog URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The dialog URL.</returns>
    public ResponsiveFileManagerOptions WithDialogUrl(string url)
    {
        DialogUrl = url;
        return this;
    }

    /// <summary>
    /// Uses the relative urls.
    /// </summary>
    /// <param name="relative">The relative.</param>
    /// <returns>The relative urls.</returns>
    public ResponsiveFileManagerOptions UseRelativeUrls(bool relative = true)
    {
        RelativeUrls = relative;
        return this;
    }

    /// <summary>
    /// Uses the cross domain.
    /// </summary>
    /// <param name="enabled">The enabled.</param>
    /// <returns>The cross domain.</returns>
    public ResponsiveFileManagerOptions UseCrossDomain(bool enabled = true)
    {
        CrossDomain = enabled;
        return this;
    }

    /// <summary>
    /// Returns a copy with the language.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <returns>The language.</returns>
    public ResponsiveFileManagerOptions WithLanguage(string language)
    {
        Language = language;
        return this;
    }

    /// <summary>
    /// Uses the host session.
    /// </summary>
    /// <returns>The host session.</returns>
    public ResponsiveFileManagerOptions UseHostSession()
    {
        HostOwnsSession = true;
        return this;
    }

    /// <summary>
    /// Gets the server.
    /// </summary>
    /// <param name="configure">The configure.</param>
    /// <returns>The server.</returns>
    public ResponsiveFileManagerOptions Server(Action<global::ResponsiveFileManager.ResponsiveFileManagerOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        ConfigureServer = configure;
        return this;
    }
}

/// <summary>
/// Represents the responsive file manager adapter.
/// </summary>
public sealed class ResponsiveFileManagerAdapter : IFileManager, IPickstackAssetSource
{
    /// <summary>
    /// The key constant.
    /// </summary>
    public const string Key = "ResponsiveFileManager";

    private readonly ResponsiveFileManagerOptions _options;

    /// <summary>
    /// Initializes a new instance of the ResponsiveFileManagerAdapter class.
    /// </summary>
    /// <param name="options">The options.</param>
    public ResponsiveFileManagerAdapter(IOptions<ResponsiveFileManagerOptions> options)
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
    public string ProviderScriptUrl => "/pickstack/assets/responsive-file-manager.js";

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
        dialogUrl = _options.DialogUrl,
        relativeUrls = _options.RelativeUrls,
        crossDomain = _options.CrossDomain,
        language = _options.Language
    });

    /// <summary>
    /// Gets the assets.
    /// </summary>
    /// <returns>The assets.</returns>
    public IEnumerable<PickstackAsset> GetAssets() => [
        PickstackAssets.Script(typeof(ResponsiveFileManagerAdapter).Assembly, "responsive-file-manager.js", "responsive-file-manager.js")
    ];
}