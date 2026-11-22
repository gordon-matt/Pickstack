namespace Pickstack.RichTextEditor.Froala;

/// <summary>
/// Site-wide Froala settings. Fluent methods return the same instance so they can be chained
/// from <c>AddFroala5</c>. A field's <c>height</c>, <c>min-height</c>, or <c>max-height</c> attribute overrides the matching setting for that editor.
/// Image, video, and file buttons stay wired to Pickstack.
/// </summary>
public sealed class FroalaOptions
{
    /// <summary>Name shown for the registered editor, such as Froala 5.</summary>
    public string Name { get; set; } = "Froala";

    /// <summary>CDN (or other) URL of the editor script, loaded first.</summary>
    public string ScriptUrl { get; set; } = "https://cdn.jsdelivr.net/npm/froala-editor@5.5.0/js/froala_editor.pkgd.min.js";

    /// <summary>URL of the script shipped in the version package. Used when <see cref="ScriptUrl"/> fails to load.</summary>
    public string? FallbackUrl { get; set; }

    /// <summary>CDN (or other) URL of the editor stylesheet.</summary>
    public string StyleUrl { get; set; } = "https://cdn.jsdelivr.net/npm/froala-editor@5.5.0/css/froala_editor.pkgd.min.css";

    /// <summary>URL of the stylesheet shipped in the version package. Used when <see cref="StyleUrl"/> fails to load.</summary>
    public string? FallbackStyleUrl { get; set; }

    /// <summary>Editor height in pixels. The default is 420. A field can override it.</summary>
    public int Height { get; set; } = 420;

    /// <summary>
    /// Minimum editor height in pixels. When set, Froala grows with the content and will not shrink below this.
    /// Leave unset to keep Froala's own default.
    /// </summary>
    public int? MinHeight { get; set; }

    /// <summary>
    /// Maximum editor height in pixels. Content scrolls once it is taller than this.
    /// Leave unset for no maximum.
    /// </summary>
    public int? MaxHeight { get; set; }

    /// <summary>Froala license key. Leave empty for an unlicensed editor.</summary>
    public string? LicenseKey { get; set; }

    /// <summary>Plugin names passed as <c>pluginsEnabled</c>. The default includes the formatting, media, table, and code plugins.</summary>
    public List<string> Plugins { get; set; } =
    [
        "align",
        "charCounter",
        "codeBeautifier",
        "codeView",
        "colors",
        "draggable",
        "emoticons",
        "entities",
        "file",
        "fontFamily",
        "fontSize",
        "fullscreen",
        "help",
        "image",
        "inlineStyle",
        "lineBreaker",
        "lineHeight",
        "link",
        "lists",
        "paragraphFormat",
        "paragraphStyle",
        "quickInsert",
        "quote",
        "specialCharacters",
        "table",
        "url",
        "video",
        "wordPaste"
    ];

    /// <summary>Toolbar buttons passed as <c>toolbarButtons</c>. <c>|</c> is a separator.</summary>
    public List<string> Toolbar { get; set; } =
    [
        "bold",
        "italic",
        "underline",
        "strikeThrough",
        "|",
        "formatOL",
        "formatUL",
        "|",
        "alignLeft",
        "alignCenter",
        "alignRight",
        "|",
        "insertLink",
        "insertImage",
        "insertVideo",
        "insertFile",
        "|",
        "html",
        "undo",
        "redo"
    ];

    /// <summary>Extra Froala init values. Structured settings are applied after these, and file picking stays wired to Pickstack.</summary>
    public Dictionary<string, object?> CustomOptions { get; } = [];

    /// <summary>Sets <see cref="ScriptUrl"/> and returns this instance.</summary>
    /// <param name="url">The script address loaded first.</param>
    /// <returns>This instance.</returns>
    public FroalaOptions WithScript(string url)
    {
        ScriptUrl = url;
        return this;
    }

    /// <summary>Sets <see cref="FallbackUrl"/> and returns this instance.</summary>
    /// <param name="url">The local script. <see langword="null"/> leaves no fallback.</param>
    /// <returns>This instance.</returns>
    public FroalaOptions WithFallback(string? url)
    {
        FallbackUrl = url;
        return this;
    }

    /// <summary>Sets <see cref="StyleUrl"/> and returns this instance.</summary>
    /// <param name="url">The stylesheet address loaded first.</param>
    /// <returns>This instance.</returns>
    public FroalaOptions WithStyle(string url)
    {
        StyleUrl = url;
        return this;
    }

    /// <summary>Sets <see cref="FallbackStyleUrl"/> and returns this instance.</summary>
    /// <param name="url">The local stylesheet. <see langword="null"/> leaves no fallback.</param>
    /// <returns>This instance.</returns>
    public FroalaOptions WithStyleFallback(string? url)
    {
        FallbackStyleUrl = url;
        return this;
    }

    /// <summary>Sets <see cref="Height"/> and returns this instance.</summary>
    /// <param name="pixels">The height, in pixels.</param>
    /// <returns>This instance.</returns>
    public FroalaOptions WithHeight(int pixels)
    {
        Height = pixels;
        return this;
    }

    /// <summary>Sets the minimum editor height, in pixels.</summary>
    /// <param name="pixels">The pixels.</param>
    /// <returns>The same options instance.</returns>
    public FroalaOptions WithMinHeight(int pixels)
    {
        MinHeight = pixels;
        return this;
    }

    /// <summary>Sets the maximum editor height, in pixels.</summary>
    /// <param name="pixels">The pixels.</param>
    /// <returns>The same options instance.</returns>
    public FroalaOptions WithMaxHeight(int pixels)
    {
        MaxHeight = pixels;
        return this;
    }

    /// <summary>Sets <see cref="LicenseKey"/> and returns this instance. A blank key is not sent, which leaves the editor unlicensed.</summary>
    /// <param name="licenseKey">The Froala license key.</param>
    /// <returns>This instance.</returns>
    public FroalaOptions WithLicenseKey(string licenseKey)
    {
        LicenseKey = licenseKey;
        return this;
    }

    /// <summary>Replaces the plugin list and returns this instance.</summary>
    /// <param name="plugins">The plugin names.</param>
    /// <returns>This instance.</returns>
    public FroalaOptions WithPlugins(params string[] plugins)
    {
        Plugins = [.. plugins];
        return this;
    }

    /// <summary>Adds plugins that are not already listed and returns this instance. The comparison ignores case.</summary>
    /// <param name="plugins">The plugin names to add.</param>
    /// <returns>This instance.</returns>
    public FroalaOptions AddPlugins(params string[] plugins)
    {
        foreach (string plugin in plugins)
        {
            if (!Plugins.Contains(plugin, StringComparer.OrdinalIgnoreCase))
            {
                Plugins.Add(plugin);
            }
        }

        return this;
    }

    /// <summary>Replaces the toolbar and returns this instance. <c>|</c> is a separator.</summary>
    /// <param name="buttons">The buttons, in order.</param>
    /// <returns>This instance.</returns>
    public FroalaOptions WithToolbar(params string[] buttons)
    {
        Toolbar = [.. buttons];
        return this;
    }

    /// <summary>Adds toolbar buttons that are not already listed and returns this instance. The comparison is case-sensitive, so <c>|</c> can be added once.</summary>
    /// <param name="buttons">The buttons to add.</param>
    /// <returns>This instance.</returns>
    public FroalaOptions AddToolbarButtons(params string[] buttons)
    {
        foreach (string button in buttons)
        {
            if (!Toolbar.Contains(button, StringComparer.Ordinal))
            {
                Toolbar.Add(button);
            }
        }

        return this;
    }

    /// <summary>
    /// Stores one Froala init value and returns this instance.
    /// A structured setting written later, such as the toolbar, replaces the same name. Image, video, and file upload stay off so picking goes through Pickstack.
    /// </summary>
    /// <param name="name">The init property name.</param>
    /// <param name="value">The value passed through to Froala.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or white space.</exception>
    public FroalaOptions Set(string name, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        CustomOptions[name] = value;
        return this;
    }

    internal Dictionary<string, object?> CreateInit()
    {
        var init = new Dictionary<string, object?>(CustomOptions)
        {
            ["height"] = Height,
            ["pluginsEnabled"] = Plugins,
            ["toolbarButtons"] = Toolbar,
            ["imageUpload"] = false,
            ["videoUpload"] = false,
            ["fileUpload"] = false
        };

        if (MinHeight is int minHeight)
        {
            init["heightMin"] = minHeight;
        }

        if (MaxHeight is int maxHeight)
        {
            init["heightMax"] = maxHeight;
        }

        if (!string.IsNullOrWhiteSpace(LicenseKey))
        {
            init["key"] = LicenseKey;
        }

        return init;
    }
}