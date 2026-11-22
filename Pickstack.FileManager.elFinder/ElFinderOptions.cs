using elFinder.NetCore.Ffmpeg;

namespace Pickstack.FileManager.elFinder;

/// <summary>
/// Site-wide elFinder client settings. The connector URL and upload folder stay owned by Pickstack.
/// </summary>
public sealed class ElFinderOptions
{
    /// <summary>Address of <c>elfinder.min.css</c>. The default is the 2.1.65 copy on cdnjs.</summary>
    public string StyleUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/css/elfinder.min.css";

    /// <summary>Address of the elFinder theme stylesheet. The default is the 2.1.65 theme on cdnjs.</summary>
    public string ThemeUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/css/theme.css";

    /// <summary>Address of <c>elfinder.min.js</c>. The default is the 2.1.65 copy on cdnjs.</summary>
    public string ScriptUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/js/elfinder.min.js";

    /// <summary>Address of the jQuery UI stylesheet elFinder's dialog needs. The default is jQuery UI 1.13.2 on cdnjs.</summary>
    public string JQueryUiStyleUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.13.2/themes/base/jquery-ui.min.css";

    /// <summary>Address of the jQuery UI script. The page must already have loaded jQuery. The default is jQuery UI 1.13.2 on cdnjs.</summary>
    public string JQueryUiScriptUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.13.2/jquery-ui.min.js";

    /// <summary>elFinder client directory. It must contain img, css, and sounds. A missing trailing slash is added when the provider configuration is built.</summary>
    public string BaseUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/";

    /// <summary>Minimum dialog height, in pixels. The default is 560.</summary>
    public int MinimumHeight { get; set; } = 560;

    /// <summary><see langword="true"/> reopens the directory that was open last time. The default is <see langword="false"/>.</summary>
    public bool RememberLastDir { get; set; }

    /// <summary><see langword="true"/> lets the dialog be resized. The default is <see langword="false"/>.</summary>
    public bool Resizable { get; set; }

    /// <summary><see langword="true"/> writes the open directory into the browser history. The default is <see langword="false"/>.</summary>
    public bool UseBrowserHistory { get; set; }

    /// <summary>Toolbar groups. Each array is one cluster of command names. The default is the navigation, upload, file, and search groups.</summary>
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

    /// <summary>Commands elFinder does not offer. The default removes callback, chmod, editor, netmount, ping, and help.</summary>
    public List<string> DisabledCommands { get; set; } = ["callback", "chmod", "editor", "netmount", "ping", "help"];

    /// <summary>When set, startup looks for ffmpeg so video files can have thumbnails.</summary>
    public bool VideoThumbnails { get; private set; }

    /// <summary>Adjusts the ffmpeg search. Download stays on unless this turns it off.</summary>
    internal Action<FfmpegBootstrapOptions>? ConfigureVideo { get; private set; }

    /// <summary>Sets <see cref="StyleUrl"/> and returns this instance.</summary>
    /// <param name="url">Address of the stylesheet.</param>
    /// <returns>This instance.</returns>
    public ElFinderOptions WithStyle(string url)
    {
        StyleUrl = url;
        return this;
    }

    /// <summary>Sets <see cref="ThemeUrl"/> and returns this instance.</summary>
    /// <param name="url">Address of the theme stylesheet.</param>
    /// <returns>This instance.</returns>
    public ElFinderOptions WithTheme(string url)
    {
        ThemeUrl = url;
        return this;
    }

    /// <summary>Sets <see cref="ScriptUrl"/> and returns this instance.</summary>
    /// <param name="url">Address of <c>elfinder.min.js</c>.</param>
    /// <returns>This instance.</returns>
    public ElFinderOptions WithScript(string url)
    {
        ScriptUrl = url;
        return this;
    }

    /// <summary>Sets <see cref="BaseUrl"/> and returns this instance.</summary>
    /// <param name="url">The client directory.</param>
    /// <returns>This instance.</returns>
    public ElFinderOptions WithBaseUrl(string url)
    {
        BaseUrl = url;
        return this;
    }

    /// <summary>Sets <see cref="MinimumHeight"/> and returns this instance.</summary>
    /// <param name="pixels">The height, in pixels.</param>
    /// <returns>This instance.</returns>
    public ElFinderOptions WithMinimumHeight(int pixels)
    {
        MinimumHeight = pixels;
        return this;
    }

    /// <summary>Sets <see cref="RememberLastDir"/> and returns this instance.</summary>
    /// <param name="remember"><see langword="false"/> turns the behavior off. The default argument is <see langword="true"/>.</param>
    /// <returns>This instance.</returns>
    public ElFinderOptions RememberLastDirectory(bool remember = true)
    {
        RememberLastDir = remember;
        return this;
    }

    /// <summary>Sets <see cref="Resizable"/> and returns this instance.</summary>
    /// <param name="allow"><see langword="false"/> keeps the dialog fixed. The default argument is <see langword="true"/>.</param>
    /// <returns>This instance.</returns>
    public ElFinderOptions AllowResize(bool allow = true)
    {
        Resizable = allow;
        return this;
    }

    /// <summary>Sets <see cref="UseBrowserHistory"/> and returns this instance.</summary>
    /// <param name="enabled"><see langword="false"/> leaves the address alone. The default argument is <see langword="true"/>.</param>
    /// <returns>This instance.</returns>
    public ElFinderOptions EnableBrowserHistory(bool enabled = true)
    {
        UseBrowserHistory = enabled;
        return this;
    }

    /// <summary>Replaces the toolbar and returns this instance. Each group is one cluster of buttons.</summary>
    /// <param name="groups">The command groups, in order.</param>
    /// <returns>This instance.</returns>
    public ElFinderOptions WithToolbar(params string[][] groups)
    {
        Toolbar = [.. groups];
        return this;
    }

    /// <summary>Appends one toolbar group and returns this instance.</summary>
    /// <param name="commands">The command names in the new group.</param>
    /// <returns>This instance.</returns>
    public ElFinderOptions AddToolbarGroup(params string[] commands)
    {
        Toolbar.Add(commands);
        return this;
    }

    /// <summary>Adds commands to <see cref="DisabledCommands"/> and returns this instance. A command already listed is left once. The comparison ignores case.</summary>
    /// <param name="commands">The command names to disable.</param>
    /// <returns>This instance.</returns>
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

    /// <summary>Removes commands from <see cref="DisabledCommands"/> and returns this instance. The comparison ignores case.</summary>
    /// <param name="commands">The command names to allow again.</param>
    /// <returns>This instance.</returns>
    public ElFinderOptions WithCommandsEnabled(params string[] commands)
    {
        DisabledCommands.RemoveAll(command => commands.Contains(command, StringComparer.OrdinalIgnoreCase));
        return this;
    }

    /// <summary>
    /// Enables video thumbnails. ffmpeg is downloaded into an <c>ffmpeg</c> folder under the
    /// content root when it is not already installed. That build is GPL-licensed.
    /// A missing ffmpeg does not stop the app; video files keep the generic icon.
    /// </summary>
    /// <param name="configure">Adjusts the ffmpeg search. <see langword="null"/> keeps the search that downloads a GPL build into an <c>ffmpeg</c> folder under the content root.</param>
    /// <returns>This instance.</returns>
    public ElFinderOptions WithVideoThumbnails(Action<FfmpegBootstrapOptions>? configure = null)
    {
        VideoThumbnails = true;
        ConfigureVideo = configure;
        return this;
    }
}