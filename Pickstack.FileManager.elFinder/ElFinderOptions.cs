namespace Pickstack.FileManager.elFinder;

public sealed class ElFinderOptions
{
    public string StyleUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/css/elfinder.min.css";

    public string ThemeUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/css/theme.css";

    public string ScriptUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/js/elfinder.min.js";

    public string JQueryUiStyleUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.13.2/themes/base/jquery-ui.min.css";

    public string JQueryUiScriptUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.13.2/jquery-ui.min.js";

    /// <summary>elFinder client directory. Must end with a slash and contain img, css, and sounds.</summary>
    public string BaseUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/elfinder/2.1.65/";
}