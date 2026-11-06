namespace Pickstack.FileManager.WebFilez;

/// <summary>
/// Site-wide WebFilez settings. The upload folder stays owned by Pickstack.
/// </summary>
public sealed class WebFilezFileManagerOptions
{
    /// <summary>WebFilez root id for the shared upload folder.</summary>
    public string RootId { get; set; } = "media";

    /// <summary>
    /// Gets or sets the root name.
    /// </summary>
    public string RootName { get; set; } = "Media";

    /// <summary><c>light</c>, <c>dark</c>, <c>auto</c>, or <c>bootstrap</c>.</summary>
    public string Theme { get; set; } = "auto";

    /// <summary>IETF language tag, such as <c>fr-CA</c>. Empty follows the UI culture.</summary>
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the select label.
    /// </summary>
    public string SelectLabel { get; set; } = "Select";

    /// <summary>
    /// Gets or sets a value indicating whether this instance is show tree.
    /// </summary>
    public bool ShowTree { get; set; } = true;

    /// <summary>The host already calls <c>MapBlazorHub</c>. Pickstack then leaves that circuit in place.</summary>
    public bool HostOwnsBlazor { get; private set; }

    /// <summary>The host already calls <c>MapWebFilezApi</c>.</summary>
    public bool HostOwnsApi { get; private set; }

    internal Action<global::WebFilez.Services.WebFilezOptions>? ConfigureServer { get; private set; }

    /// <summary>
    /// Returns a copy with the root.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <returns>The root.</returns>
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

    /// <summary>
    /// Returns a copy with the theme.
    /// </summary>
    /// <param name="theme">The theme.</param>
    /// <returns>The theme.</returns>
    public WebFilezFileManagerOptions WithTheme(string theme)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(theme);
        Theme = theme;
        return this;
    }

    /// <summary>
    /// Returns a copy with the language.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <returns>The language.</returns>
    public WebFilezFileManagerOptions WithLanguage(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        Language = language;
        return this;
    }

    /// <summary>
    /// Returns a copy with the select label.
    /// </summary>
    /// <param name="label">The label.</param>
    /// <returns>The select label.</returns>
    public WebFilezFileManagerOptions WithSelectLabel(string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        SelectLabel = label;
        return this;
    }

    /// <summary>
    /// Gets the hide tree.
    /// </summary>
    /// <returns>The hide tree.</returns>
    public WebFilezFileManagerOptions HideTree()
    {
        ShowTree = false;
        return this;
    }

    /// <summary>
    /// Uses the host blazor.
    /// </summary>
    /// <returns>The host blazor.</returns>
    public WebFilezFileManagerOptions UseHostBlazor()
    {
        HostOwnsBlazor = true;
        return this;
    }

    /// <summary>
    /// Uses the host API.
    /// </summary>
    /// <returns>The host API.</returns>
    public WebFilezFileManagerOptions UseHostApi()
    {
        HostOwnsApi = true;
        return this;
    }

    /// <summary>Configures WebFilez itself: upload limits, blocked extensions, and extra roots.</summary>
    public WebFilezFileManagerOptions Server(Action<global::WebFilez.Services.WebFilezOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        ConfigureServer = configure;
        return this;
    }
}
