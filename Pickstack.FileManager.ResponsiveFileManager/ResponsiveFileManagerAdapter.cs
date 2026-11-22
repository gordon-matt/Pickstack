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
    /// <summary>Address of the dialog page. The default is <c>/filemanager/dialog.php</c>.</summary>
    public string DialogUrl { get; set; } = "/filemanager/dialog.php";

    /// <summary><see langword="true"/> asks the dialog for root-relative URLs. The default is <see langword="false"/>.</summary>
    public bool RelativeUrls { get; set; }

    /// <summary><see langword="true"/> allows the dialog to answer a page on another origin. The default is <see langword="false"/>.</summary>
    public bool CrossDomain { get; set; }

    /// <summary>Dialog language, such as <c>en_EN</c>. <see langword="null"/> leaves the package default.</summary>
    public string? Language { get; set; }

    /// <summary>
    /// The host already calls <c>UseSession</c>. Pickstack then only enables the file manager middleware.
    /// </summary>
    public bool HostOwnsSession { get; private set; }

    internal Action<global::ResponsiveFileManager.ResponsiveFileManagerOptions>? ConfigureServer { get; private set; }

    /// <summary>Sets <see cref="DialogUrl"/> and returns this instance.</summary>
    /// <param name="url">Address of the dialog page.</param>
    /// <returns>This instance.</returns>
    public ResponsiveFileManagerOptions WithDialogUrl(string url)
    {
        DialogUrl = url;
        return this;
    }

    /// <summary>Sets <see cref="RelativeUrls"/> and returns this instance.</summary>
    /// <param name="relative"><see langword="false"/> leaves the dialog's own URL form. The default argument is <see langword="true"/>.</param>
    /// <returns>This instance.</returns>
    public ResponsiveFileManagerOptions UseRelativeUrls(bool relative = true)
    {
        RelativeUrls = relative;
        return this;
    }

    /// <summary>Sets <see cref="CrossDomain"/> and returns this instance.</summary>
    /// <param name="enabled"><see langword="false"/> keeps the dialog on the same origin. The default argument is <see langword="true"/>.</param>
    /// <returns>This instance.</returns>
    public ResponsiveFileManagerOptions UseCrossDomain(bool enabled = true)
    {
        CrossDomain = enabled;
        return this;
    }

    /// <summary>Sets <see cref="Language"/> and returns this instance.</summary>
    /// <param name="language">The dialog language, such as <c>en_EN</c>.</param>
    /// <returns>This instance.</returns>
    public ResponsiveFileManagerOptions WithLanguage(string language)
    {
        Language = language;
        return this;
    }

    /// <summary>
    /// Leaves session middleware to the host. Pickstack then does not call <c>AddSession</c> or <c>UseSession</c>, and still enables the file manager middleware.
    /// </summary>
    /// <returns>This instance.</returns>
    public ResponsiveFileManagerOptions UseHostSession()
    {
        HostOwnsSession = true;
        return this;
    }

    /// <summary>
    /// Configures the PHP package after Pickstack has set the upload and thumbnail paths.
    /// The callback can replace those paths. Upload limits belong here.
    /// </summary>
    /// <param name="configure">The PHP options callback.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configure"/> is <see langword="null"/>.</exception>
    public ResponsiveFileManagerOptions Server(Action<global::ResponsiveFileManager.ResponsiveFileManagerOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        ConfigureServer = configure;
        return this;
    }
}

/// <summary>Responsive File Manager as <see cref="IFileManager"/>. The dialog is the package's PHP page.</summary>
public sealed class ResponsiveFileManagerAdapter : IFileManager, IPickstackAssetSource
{
    /// <summary>The configuration name and the value of <see cref="Name"/>. The value is <c>ResponsiveFileManager</c>.</summary>
    public const string Key = "ResponsiveFileManager";

    private readonly ResponsiveFileManagerOptions _options;

    /// <summary>Reads the dialog options.</summary>
    /// <param name="options">The dialog options.</param>
    public ResponsiveFileManagerAdapter(IOptions<ResponsiveFileManagerOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>The display name. The value is <see cref="Key"/>.</summary>
    public string Name => Key;

    /// <summary>The embedded provider script. The value is <c>/pickstack/assets/responsive-file-manager.js</c>.</summary>
    public string ProviderScriptUrl => "/pickstack/assets/responsive-file-manager.js";

    /// <summary>No extra styles or scripts. The PHP dialog brings its own.</summary>
    /// <returns>An empty list.</returns>
    public IReadOnlyList<ClientAsset> GetClientAssets() => [];

    /// <summary>JSON for the provider script: the dialog address, relative URLs, cross-domain, and language.</summary>
    /// <returns>The JSON.</returns>
    public string GetProviderConfiguration() => JsonSerializer.Serialize(new
    {
        dialogUrl = _options.DialogUrl,
        relativeUrls = _options.RelativeUrls,
        crossDomain = _options.CrossDomain,
        language = _options.Language
    });

    /// <summary>The embedded <c>responsive-file-manager.js</c> provider script.</summary>
    /// <returns>The asset.</returns>
    public IEnumerable<PickstackAsset> GetAssets() => [
        PickstackAssets.Script(typeof(ResponsiveFileManagerAdapter).Assembly, "responsive-file-manager.js", "responsive-file-manager.js")
    ];
}