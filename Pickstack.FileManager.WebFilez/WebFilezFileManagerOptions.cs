namespace Pickstack.FileManager.WebFilez;

/// <summary>
/// Site-wide WebFilez settings. The upload folder stays owned by Pickstack.
/// </summary>
public sealed class WebFilezFileManagerOptions
{
    /// <summary>WebFilez root id for the shared upload folder.</summary>
    public string RootId { get; set; } = "media";

    /// <summary>Name shown for the shared upload folder. The default is Media.</summary>
    public string RootName { get; set; } = "Media";

    /// <summary><c>light</c>, <c>dark</c>, <c>auto</c>, or <c>bootstrap</c>.</summary>
    public string Theme { get; set; } = "auto";

    /// <summary>IETF language tag, such as <c>fr-CA</c>. Empty follows the UI culture.</summary>
    public string? Language { get; set; }

    /// <summary>Text of the button that chooses the open file. The default is Select.</summary>
    public string SelectLabel { get; set; } = "Select";

    /// <summary><see langword="true"/> shows the folder tree. The default is <see langword="true"/>.</summary>
    public bool ShowTree { get; set; } = true;

    /// <summary>The host already calls <c>MapBlazorHub</c>. Pickstack then leaves that circuit in place.</summary>
    public bool HostOwnsBlazor { get; private set; }

    /// <summary>The host already calls <c>MapWebFilezApi</c>.</summary>
    public bool HostOwnsApi { get; private set; }

    internal Action<global::WebFilez.Services.WebFilezOptions>? ConfigureServer { get; private set; }

    /// <summary>Sets the shared root id and, when <paramref name="name"/> is not blank, its display name. Returns this instance.</summary>
    /// <param name="id">The WebFilez root id.</param>
    /// <param name="name">The display name. <see langword="null"/> or white space leaves <see cref="RootName"/> unchanged.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentException"><paramref name="id"/> is <see langword="null"/> or white space.</exception>
    public WebFilezFileManagerOptions WithRoot(string id, string? name = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        RootId = id;
        if (!string.IsNullOrWhiteSpace(name))
        {
            RootName = name;
        }

        return this;
    }

    /// <summary>Sets <see cref="Theme"/> and returns this instance.</summary>
    /// <param name="theme"><c>light</c>, <c>dark</c>, <c>auto</c>, or <c>bootstrap</c>.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentException"><paramref name="theme"/> is <see langword="null"/> or white space.</exception>
    public WebFilezFileManagerOptions WithTheme(string theme)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(theme);
        Theme = theme;
        return this;
    }

    /// <summary>Sets <see cref="Language"/> and returns this instance.</summary>
    /// <param name="language">An IETF language tag, such as <c>fr-CA</c>.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentException"><paramref name="language"/> is <see langword="null"/> or white space.</exception>
    public WebFilezFileManagerOptions WithLanguage(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        Language = language;
        return this;
    }

    /// <summary>Sets <see cref="SelectLabel"/> and returns this instance.</summary>
    /// <param name="label">The button text.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentException"><paramref name="label"/> is <see langword="null"/> or white space.</exception>
    public WebFilezFileManagerOptions WithSelectLabel(string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        SelectLabel = label;
        return this;
    }

    /// <summary>Hides the folder tree and returns this instance.</summary>
    /// <returns>This instance.</returns>
    public WebFilezFileManagerOptions HideTree()
    {
        ShowTree = false;
        return this;
    }

    /// <summary>Leaves <c>MapBlazorHub</c> to the host and returns this instance. Blazor services are still registered.</summary>
    /// <returns>This instance.</returns>
    public WebFilezFileManagerOptions UseHostBlazor()
    {
        HostOwnsBlazor = true;
        return this;
    }

    /// <summary>Leaves <c>MapWebFilezApi</c> to the host and returns this instance.</summary>
    /// <returns>This instance.</returns>
    public WebFilezFileManagerOptions UseHostApi()
    {
        HostOwnsApi = true;
        return this;
    }

    /// <summary>
    /// Configures WebFilez itself: upload limits, blocked extensions, and extra roots. Returns this instance.
    /// The adapter does not read a WebFilez configuration section, so extra roots are declared here.
    /// The shared <c>media</c> root is still added, and an extra root with the same id is skipped.
    /// </summary>
    /// <param name="configure">The WebFilez options callback.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configure"/> is <see langword="null"/>.</exception>
    public WebFilezFileManagerOptions Server(Action<global::WebFilez.Services.WebFilezOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        ConfigureServer = configure;
        return this;
    }
}
