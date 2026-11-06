namespace Pickstack.RichTextEditor.TinyMce;

/// <summary>
/// Site-wide TinyMCE settings. Fluent methods return the same instance so they can be chained
/// from <c>AddTinyMce5</c>, <c>AddTinyMce6</c>, <c>AddTinyMce7</c>, or <c>AddTinyMce8</c>.
/// A field's <c>height</c> attribute overrides <see cref="Height"/> for that editor.
/// </summary>
public sealed class TinyMceOptions
{
    /// <summary>Name shown for the registered editor, such as TinyMCE 8.</summary>
    public string Name { get; set; } = "TinyMCE";

    /// <summary>CDN (or other) URL loaded first.</summary>
    public string ScriptUrl { get; set; } = "https://cdn.jsdelivr.net/npm/tinymce@8.10.1/tinymce.min.js";

    /// <summary>URL of the copy shipped in the version package. Used when <see cref="ScriptUrl"/> fails to load.</summary>
    public string? FallbackUrl { get; set; }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    public int Height { get; set; } = 420;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is menubar.
    /// </summary>
    public bool Menubar { get; set; }

    /// <summary>
    /// When set, <see cref="LicenseKey"/> and <see cref="Promotion"/> are sent to TinyMCE.
    /// TinyMCE 7 and 8 need this. TinyMCE 5 and 6 do not.
    /// </summary>
    public bool IncludeLicense { get; set; } = true;

    /// <summary>
    /// Gets or sets the license key.
    /// </summary>
    public string LicenseKey { get; set; } = "gpl";

    /// <summary>
    /// Gets or sets a value indicating whether this instance is promotion.
    /// </summary>
    public bool Promotion { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is relative urls.
    /// </summary>
    public bool RelativeUrls { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is remove script host.
    /// </summary>
    public bool RemoveScriptHost { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is convert urls.
    /// </summary>
    public bool ConvertUrls { get; set; } = true;

    /// <summary>
    /// Gets or sets the file picker types.
    /// </summary>
    public string FilePickerTypes { get; set; } = "file image media";

    /// <summary>
    /// Gets or sets the plugins.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the toolbar.
    /// </summary>
    public string Toolbar { get; set; } = "undo redo | bold italic underline | alignleft aligncenter alignright | bullist numlist | link image media | code";

    /// <summary>Extra TinyMCE init values. Structured settings are applied after these, and the file picker stays wired to Pickstack.</summary>
    public Dictionary<string, object?> CustomOptions { get; } = [];

    /// <summary>
    /// Returns a copy with the script.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The script.</returns>
    public TinyMceOptions WithScript(string url)
    {
        ScriptUrl = url;
        return this;
    }

    /// <summary>
    /// Returns a copy with the fallback.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The fallback.</returns>
    public TinyMceOptions WithFallback(string? url)
    {
        FallbackUrl = url;
        return this;
    }

    /// <summary>
    /// Returns a copy with the height.
    /// </summary>
    /// <param name="pixels">The pixels.</param>
    /// <returns>The height.</returns>
    public TinyMceOptions WithHeight(int pixels)
    {
        Height = pixels;
        return this;
    }

    /// <summary>
    /// Gets the show menubar.
    /// </summary>
    /// <param name="show">The show.</param>
    /// <returns>The show menubar.</returns>
    public TinyMceOptions ShowMenubar(bool show = true)
    {
        Menubar = show;
        return this;
    }

    /// <summary>
    /// Returns a copy with the license key.
    /// </summary>
    /// <param name="licenseKey">The license key.</param>
    /// <returns>The license key.</returns>
    public TinyMceOptions WithLicenseKey(string licenseKey)
    {
        LicenseKey = licenseKey;
        IncludeLicense = true;
        return this;
    }

    /// <summary>
    /// Gets the show promotion.
    /// </summary>
    /// <param name="show">The show.</param>
    /// <returns>The show promotion.</returns>
    public TinyMceOptions ShowPromotion(bool show = true)
    {
        Promotion = show;
        return this;
    }

    /// <summary>
    /// Uses the relative urls.
    /// </summary>
    /// <param name="relative">The relative.</param>
    /// <returns>The relative urls.</returns>
    public TinyMceOptions UseRelativeUrls(bool relative = true)
    {
        RelativeUrls = relative;
        return this;
    }

    /// <summary>
    /// Returns a copy with the file picker types.
    /// </summary>
    /// <param name="types">The types.</param>
    /// <returns>The file picker types.</returns>
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

    /// <summary>
    /// Returns a copy with the toolbar.
    /// </summary>
    /// <param name="toolbar">The toolbar.</param>
    /// <returns>The toolbar.</returns>
    public TinyMceOptions WithToolbar(string toolbar)
    {
        Toolbar = toolbar;
        return this;
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns>The tiny mce options.</returns>
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