namespace Pickstack.RichTextEditor.Froala;

/// <summary>
/// Site-wide Froala settings. Fluent methods return the same instance so they can be chained
/// from <c>AddFroala5</c>. A field's <c>height</c> attribute overrides <see cref="Height"/> for that editor.
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

    public int Height { get; set; } = 420;

    /// <summary>Froala license key. Leave empty for an unlicensed editor.</summary>
    public string? LicenseKey { get; set; }

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

    public FroalaOptions WithScript(string url)
    {
        ScriptUrl = url;
        return this;
    }

    public FroalaOptions WithFallback(string? url)
    {
        FallbackUrl = url;
        return this;
    }

    public FroalaOptions WithStyle(string url)
    {
        StyleUrl = url;
        return this;
    }

    public FroalaOptions WithStyleFallback(string? url)
    {
        FallbackStyleUrl = url;
        return this;
    }

    public FroalaOptions WithHeight(int pixels)
    {
        Height = pixels;
        return this;
    }

    public FroalaOptions WithLicenseKey(string licenseKey)
    {
        LicenseKey = licenseKey;
        return this;
    }

    /// <summary>Replaces the plugin list.</summary>
    public FroalaOptions WithPlugins(params string[] plugins)
    {
        Plugins = [.. plugins];
        return this;
    }

    /// <summary>Adds plugins that are not already listed.</summary>
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

    /// <summary>Replaces the toolbar. <c>|</c> is a separator.</summary>
    public FroalaOptions WithToolbar(params string[] buttons)
    {
        Toolbar = [.. buttons];
        return this;
    }

    /// <summary>Adds toolbar buttons that are not already listed.</summary>
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

        if (!string.IsNullOrWhiteSpace(LicenseKey))
        {
            init["key"] = LicenseKey;
        }

        return init;
    }
}