namespace Pickstack.FileManager.WebFilez;

/// <summary>
/// Site-wide WebFilez settings. The upload folder stays owned by Pickstack.
/// </summary>
public sealed class WebFilezFileManagerOptions
{
    /// <summary>WebFilez root id for the shared upload folder.</summary>
    public string RootId { get; set; } = "media";

    public string RootName { get; set; } = "Media";

    /// <summary><c>light</c>, <c>dark</c>, <c>auto</c>, or <c>bootstrap</c>.</summary>
    public string Theme { get; set; } = "auto";

    /// <summary>IETF language tag, such as <c>fr-CA</c>. Empty follows the UI culture.</summary>
    public string? Language { get; set; }

    public string SelectLabel { get; set; } = "Select";

    public bool ShowTree { get; set; } = true;

    /// <summary>The host already calls <c>MapBlazorHub</c>. Pickstack then leaves that circuit in place.</summary>
    public bool HostOwnsBlazor { get; private set; }

    /// <summary>The host already calls <c>MapWebFilezApi</c>.</summary>
    public bool HostOwnsApi { get; private set; }

    internal Action<global::WebFilez.Services.WebFilezOptions>? ConfigureServer { get; private set; }

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

    public WebFilezFileManagerOptions WithTheme(string theme)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(theme);
        Theme = theme;
        return this;
    }

    public WebFilezFileManagerOptions WithLanguage(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        Language = language;
        return this;
    }

    public WebFilezFileManagerOptions WithSelectLabel(string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        SelectLabel = label;
        return this;
    }

    public WebFilezFileManagerOptions HideTree()
    {
        ShowTree = false;
        return this;
    }

    public WebFilezFileManagerOptions UseHostBlazor()
    {
        HostOwnsBlazor = true;
        return this;
    }

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
