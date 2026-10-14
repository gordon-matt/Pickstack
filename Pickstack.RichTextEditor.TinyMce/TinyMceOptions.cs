namespace Pickstack.RichTextEditor.TinyMce;

/// <summary>
/// Site-wide TinyMCE settings. Fluent methods return the same instance so they can be chained
/// from <c>AddTinyMce5</c>, <c>AddTinyMce6</c>, or <c>AddTinyMce7</c>.
/// A field's <c>height</c> attribute overrides <see cref="Height"/> for that editor.
/// </summary>
public sealed class TinyMceOptions
{
    /// <summary>Name shown for the registered editor, such as TinyMCE 7.</summary>
    public string Name { get; set; } = "TinyMCE";

    /// <summary>CDN (or other) URL loaded first.</summary>
    public string ScriptUrl { get; set; } = "https://cdn.jsdelivr.net/npm/tinymce@7.9.0/tinymce.min.js";

    /// <summary>URL of the copy shipped in the version package. Used when <see cref="ScriptUrl"/> fails to load.</summary>
    public string? FallbackUrl { get; set; }

    public int Height { get; set; } = 420;

    public bool Menubar { get; set; }

    /// <summary>
    /// When set, <see cref="LicenseKey"/> and <see cref="Promotion"/> are sent to TinyMCE.
    /// TinyMCE 7 needs this. TinyMCE 5 and 6 do not.
    /// </summary>
    public bool IncludeLicense { get; set; } = true;

    public string LicenseKey { get; set; } = "gpl";

    public bool Promotion { get; set; }

    public bool RelativeUrls { get; set; }

    public bool RemoveScriptHost { get; set; } = true;

    public bool ConvertUrls { get; set; } = true;

    public string FilePickerTypes { get; set; } = "file image media";

    public List<string> Plugins { get; set; } =
    [
        "autolink",
        "lists",
        "link",
        "image",
        "media",
        "table",
        "code",
        "fullscreen"
    ];

    public string Toolbar { get; set; } = "undo redo | bold italic underline | alignleft aligncenter alignright | bullist numlist | link image media | code";

    /// <summary>Extra TinyMCE init values. Structured settings are applied after these, and the file picker stays wired to Pickstack.</summary>
    public Dictionary<string, object?> CustomOptions { get; } = [];

    public TinyMceOptions WithScript(string url)
    {
        ScriptUrl = url;
        return this;
    }

    public TinyMceOptions WithFallback(string? url)
    {
        FallbackUrl = url;
        return this;
    }

    public TinyMceOptions WithHeight(int pixels)
    {
        Height = pixels;
        return this;
    }

    public TinyMceOptions ShowMenubar(bool show = true)
    {
        Menubar = show;
        return this;
    }

    public TinyMceOptions WithLicenseKey(string licenseKey)
    {
        LicenseKey = licenseKey;
        IncludeLicense = true;
        return this;
    }

    public TinyMceOptions ShowPromotion(bool show = true)
    {
        Promotion = show;
        return this;
    }

    public TinyMceOptions UseRelativeUrls(bool relative = true)
    {
        RelativeUrls = relative;
        return this;
    }

    public TinyMceOptions WithFilePickerTypes(string types)
    {
        FilePickerTypes = types;
        return this;
    }

    /// <summary>Replaces the plugin list.</summary>
    public TinyMceOptions WithPlugins(params string[] plugins)
    {
        Plugins = [.. plugins];
        return this;
    }

    /// <summary>Adds plugins that are not already listed.</summary>
    public TinyMceOptions AddPlugins(params string[] plugins)
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

    public TinyMceOptions WithToolbar(string toolbar)
    {
        Toolbar = toolbar;
        return this;
    }

    public TinyMceOptions Set(string name, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        CustomOptions[name] = value;
        return this;
    }

    internal Dictionary<string, object?> CreateInit()
    {
        var init = new Dictionary<string, object?>(CustomOptions);
        if (IncludeLicense)
        {
            init["license_key"] = LicenseKey;
            init["promotion"] = Promotion;
        }

        init["height"] = Height;
        init["menubar"] = Menubar;
        init["plugins"] = string.Join(' ', Plugins);
        init["toolbar"] = Toolbar;
        init["relative_urls"] = RelativeUrls;
        init["remove_script_host"] = RemoveScriptHost;
        init["convert_urls"] = ConvertUrls;
        init["file_picker_types"] = FilePickerTypes;

        return init;
    }
}