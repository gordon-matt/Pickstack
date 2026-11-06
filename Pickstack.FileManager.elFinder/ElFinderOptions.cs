namespace Pickstack.FileManager.elFinder;

/// <summary>
/// Site-wide elFinder client settings. The connector URL and upload folder stay owned by Pickstack.
/// </summary>
public sealed class ElFinderOptions
{
    /// <summary>
    /// Gets or sets the style URL.
    /// </summary>
    public string StyleUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/css/elfinder.min.css";

    /// <summary>
    /// Gets or sets the theme URL.
    /// </summary>
    public string ThemeUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/css/theme.css";

    /// <summary>
    /// Gets or sets the script URL.
    /// </summary>
    public string ScriptUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/js/elfinder.min.js";

    /// <summary>
    /// Gets or sets the j query UI style URL.
    /// </summary>
    public string JQueryUiStyleUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.13.2/themes/base/jquery-ui.min.css";

    /// <summary>
    /// Gets or sets the j query UI script URL.
    /// </summary>
    public string JQueryUiScriptUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.13.2/jquery-ui.min.js";

    /// <summary>elFinder client directory. Must end with a slash and contain img, css, and sounds.</summary>
    public string BaseUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/";

    /// <summary>
    /// Gets or sets the minimum height.
    /// </summary>
    public int MinimumHeight { get; set; } = 560;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is remember last dir.
    /// </summary>
    public bool RememberLastDir { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is resizable.
    /// </summary>
    public bool Resizable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is use browser history.
    /// </summary>
    public bool UseBrowserHistory { get; set; }

    /// <summary>
    /// Gets or sets the toolbar.
    /// </summary>
    public List<string[]> Toolbar { get; set; } =
    [
        ["back", "forward"],
        ["reload"],
        ["home", "up"],
        ["mkdir", "mkfile", "upload"],
        ["open", "download", "getfile"],
        ["info"],
        ["quicklook"],
        ["copy", "cut", "paste"],
        ["rm"],
        ["duplicate", "rename", "edit"],
        ["view", "sort"],
        ["search"]
    ];

    /// <summary>
    /// Gets or sets the disabled commands.
    /// </summary>
    public List<string> DisabledCommands { get; set; } = ["callback", "chmod", "editor", "netmount", "ping", "help"];

    /// <summary>
    /// Returns a copy with the style.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The style.</returns>
    public ElFinderOptions WithStyle(string url)
    {
        StyleUrl = url;
        return this;
    }

    /// <summary>
    /// Returns a copy with the theme.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The theme.</returns>
    public ElFinderOptions WithTheme(string url)
    {
        ThemeUrl = url;
        return this;
    }

    /// <summary>
    /// Returns a copy with the script.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The script.</returns>
    public ElFinderOptions WithScript(string url)
    {
        ScriptUrl = url;
        return this;
    }

    /// <summary>
    /// Returns a copy with the base URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The base URL.</returns>
    public ElFinderOptions WithBaseUrl(string url)
    {
        BaseUrl = url;
        return this;
    }

    /// <summary>
    /// Returns a copy with the minimum height.
    /// </summary>
    /// <param name="pixels">The pixels.</param>
    /// <returns>The minimum height.</returns>
    public ElFinderOptions WithMinimumHeight(int pixels)
    {
        MinimumHeight = pixels;
        return this;
    }

    /// <summary>
    /// Gets the remember last directory.
    /// </summary>
    /// <param name="remember">The remember.</param>
    /// <returns>The remember last directory.</returns>
    public ElFinderOptions RememberLastDirectory(bool remember = true)
    {
        RememberLastDir = remember;
        return this;
    }

    /// <summary>
    /// Gets the allow resize.
    /// </summary>
    /// <param name="allow">The allow.</param>
    /// <returns>The allow resize.</returns>
    public ElFinderOptions AllowResize(bool allow = true)
    {
        Resizable = allow;
        return this;
    }

    /// <summary>
    /// Enables the browser history.
    /// </summary>
    /// <param name="enabled">The enabled.</param>
    /// <returns>The browser history.</returns>
    public ElFinderOptions EnableBrowserHistory(bool enabled = true)
    {
        UseBrowserHistory = enabled;
        return this;
    }

    /// <summary>Replaces the toolbar. Each group is one cluster of buttons.</summary>
    public ElFinderOptions WithToolbar(params string[][] groups)
    {
        Toolbar = [.. groups];
        return this;
    }

    /// <summary>
    /// Adds the toolbar group.
    /// </summary>
    /// <param name="commands">The commands.</param>
    /// <returns>The toolbar group.</returns>
    public ElFinderOptions AddToolbarGroup(params string[] commands)
    {
        Toolbar.Add(commands);
        return this;
    }

    /// <summary>
    /// Disables the commands.
    /// </summary>
    /// <param name="commands">The commands.</param>
    /// <returns>The commands.</returns>
    public ElFinderOptions DisableCommands(params string[] commands)
    {
        foreach (string command in commands)
        {
            if (!DisabledCommands.Contains(command, StringComparer.OrdinalIgnoreCase))
            {
                DisabledCommands.Add(command);
            }
        }

        return this;
    }

    /// <summary>
    /// Returns a copy with the commands enabled.
    /// </summary>
    /// <param name="commands">The commands.</param>
    /// <returns>The commands enabled.</returns>
    public ElFinderOptions WithCommandsEnabled(params string[] commands)
    {
        DisabledCommands.RemoveAll(command => commands.Contains(command, StringComparer.OrdinalIgnoreCase));
        return this;
    }
}