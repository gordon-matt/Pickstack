namespace Pickstack.FileManager.elFinder;

/// <summary>
/// Site-wide elFinder client settings. The connector URL and upload folder stay owned by Pickstack.
/// </summary>
public sealed class ElFinderOptions
{
    public string StyleUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/css/elfinder.min.css";

    public string ThemeUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/css/theme.css";

    public string ScriptUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/js/elfinder.min.js";

    public string JQueryUiStyleUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.13.2/themes/base/jquery-ui.min.css";

    public string JQueryUiScriptUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.13.2/jquery-ui.min.js";

    /// <summary>elFinder client directory. Must end with a slash and contain img, css, and sounds.</summary>
    public string BaseUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/";

    public int MinimumHeight { get; set; } = 560;

    public bool RememberLastDir { get; set; }

    public bool Resizable { get; set; }

    public bool UseBrowserHistory { get; set; }

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

    public List<string> DisabledCommands { get; set; } = ["callback", "chmod", "editor", "netmount", "ping", "help"];

    public ElFinderOptions WithStyle(string url)
    {
        StyleUrl = url;
        return this;
    }

    public ElFinderOptions WithTheme(string url)
    {
        ThemeUrl = url;
        return this;
    }

    public ElFinderOptions WithScript(string url)
    {
        ScriptUrl = url;
        return this;
    }

    public ElFinderOptions WithBaseUrl(string url)
    {
        BaseUrl = url;
        return this;
    }

    public ElFinderOptions WithMinimumHeight(int pixels)
    {
        MinimumHeight = pixels;
        return this;
    }

    public ElFinderOptions RememberLastDirectory(bool remember = true)
    {
        RememberLastDir = remember;
        return this;
    }

    public ElFinderOptions AllowResize(bool allow = true)
    {
        Resizable = allow;
        return this;
    }

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

    public ElFinderOptions AddToolbarGroup(params string[] commands)
    {
        Toolbar.Add(commands);
        return this;
    }

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

    public ElFinderOptions WithCommandsEnabled(params string[] commands)
    {
        DisabledCommands.RemoveAll(command => commands.Contains(command, StringComparer.OrdinalIgnoreCase));
        return this;
    }
}