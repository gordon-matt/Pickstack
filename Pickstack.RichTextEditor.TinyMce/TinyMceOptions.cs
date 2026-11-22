namespace Pickstack.RichTextEditor.TinyMce;

/// <summary>
/// Site-wide TinyMCE settings. Fluent methods return the same instance so they can be chained
/// from <c>AddTinyMce5</c>, <c>AddTinyMce6</c>, <c>AddTinyMce7</c>, or <c>AddTinyMce8</c>.
/// A field's <c>height</c>, <c>min-height</c>, or <c>max-height</c> attribute overrides the matching setting for that editor.
/// </summary>
public sealed class TinyMceOptions
{
    /// <summary>Name shown for the registered editor, such as TinyMCE 8.</summary>
    public string Name { get; set; } = "TinyMCE";

    /// <summary>CDN (or other) URL loaded first.</summary>
    public string ScriptUrl { get; set; } = "https://cdn.jsdelivr.net/npm/tinymce@8.10.1/tinymce.min.js";

    /// <summary>URL of the copy shipped in the version package. Used when <see cref="ScriptUrl"/> fails to load.</summary>
    public string? FallbackUrl { get; set; }

    /// <summary>Editor height in pixels. The default is 420. A field can override it.</summary>
    public int Height { get; set; } = 420;

    /// <summary>
    /// Minimum editor height in pixels. The <c>autoresize</c> plugin sizes the editor to its content
    /// and uses this value as the floor, so <see cref="Height"/> no longer holds it at a fixed size.
    /// Leave unset to keep TinyMCE's own default.
    /// </summary>
    public int? MinHeight { get; set; }

    /// <summary>
    /// Maximum editor height in pixels. With <c>autoresize</c>, the editor scrolls once the content
    /// is taller than this. Leave unset for no maximum.
    /// </summary>
    public int? MaxHeight { get; set; }

    /// <summary><see langword="true"/> shows the menu bar. The default is <see langword="false"/>.</summary>
    public bool Menubar { get; set; }

    /// <summary>
    /// When set, <see cref="LicenseKey"/> and <see cref="Promotion"/> are sent to TinyMCE.
    /// TinyMCE 7 and 8 need this. TinyMCE 5 and 6 do not.
    /// </summary>
    public bool IncludeLicense { get; set; } = true;

    /// <summary>Value sent as <c>license_key</c> when <see cref="IncludeLicense"/> is set. The default is <c>gpl</c>.</summary>
    public string LicenseKey { get; set; } = "gpl";

    /// <summary><see langword="true"/> shows the TinyMCE promotion. Sent only when <see cref="IncludeLicense"/> is set. The default is <see langword="false"/>.</summary>
    public bool Promotion { get; set; }

    /// <summary><see langword="true"/> stores URLs relative to the document. The default is <see langword="false"/>, so a same-origin file stays root-relative.</summary>
    public bool RelativeUrls { get; set; }

    /// <summary><see langword="true"/> removes the host from same-origin URLs. The default is <see langword="true"/>.</summary>
    public bool RemoveScriptHost { get; set; } = true;

    /// <summary><see langword="true"/> lets TinyMCE rewrite URLs. The default is <see langword="true"/>.</summary>
    public bool ConvertUrls { get; set; } = true;

    /// <summary>TinyMCE file picker types, separated by spaces. The default is <c>file image media</c>.</summary>
    public string FilePickerTypes { get; set; } = "file image media";

    /// <summary>Plugin names. The default is autolink, lists, link, image, media, table, code, and fullscreen.</summary>
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

    /// <summary>One toolbar row. Ignored when <see cref="Toolbars"/> has any entries.</summary>
    public string Toolbar { get; set; } = "undo redo | bold italic underline | alignleft aligncenter alignright | bullist numlist | link image media | code";

    /// <summary>
    /// Toolbar rows. When this has any entries, TinyMCE receives a <c>toolbar</c> array and <see cref="Toolbar"/> is not sent.
    /// </summary>
    public IReadOnlyList<string>? Toolbars { get; set; }

    /// <summary>Extra TinyMCE init values. Structured settings are applied after these, and the file picker stays wired to Pickstack.</summary>
    public Dictionary<string, object?> CustomOptions { get; } = [];

    /// <summary>Sets <see cref="ScriptUrl"/> and returns this instance.</summary>
    /// <param name="url">The address loaded first.</param>
    /// <returns>This instance.</returns>
    public TinyMceOptions WithScript(string url)
    {
        ScriptUrl = url;
        return this;
    }

    /// <summary>Sets <see cref="FallbackUrl"/> and returns this instance.</summary>
    /// <param name="url">The local copy. <see langword="null"/> leaves no fallback.</param>
    /// <returns>This instance.</returns>
    public TinyMceOptions WithFallback(string? url)
    {
        FallbackUrl = url;
        return this;
    }

    /// <summary>Sets <see cref="Height"/> and returns this instance.</summary>
    /// <param name="pixels">The height, in pixels.</param>
    /// <returns>This instance.</returns>
    public TinyMceOptions WithHeight(int pixels)
    {
        Height = pixels;
        return this;
    }

    /// <summary>Sets the minimum editor height, in pixels.</summary>
    /// <param name="pixels">The pixels.</param>
    /// <returns>The same options instance.</returns>
    public TinyMceOptions WithMinHeight(int pixels)
    {
        MinHeight = pixels;
        return this;
    }

    /// <summary>Sets the maximum editor height, in pixels.</summary>
    /// <param name="pixels">The pixels.</param>
    /// <returns>The same options instance.</returns>
    public TinyMceOptions WithMaxHeight(int pixels)
    {
        MaxHeight = pixels;
        return this;
    }

    /// <summary>Sets <see cref="Menubar"/> and returns this instance.</summary>
    /// <param name="show"><see langword="false"/> hides the menu bar. The default argument is <see langword="true"/>.</param>
    /// <returns>This instance.</returns>
    public TinyMceOptions ShowMenubar(bool show = true)
    {
        Menubar = show;
        return this;
    }

    /// <summary>Sets <see cref="LicenseKey"/>, turns <see cref="IncludeLicense"/> on, and returns this instance.</summary>
    /// <param name="licenseKey">The key sent as <c>license_key</c>.</param>
    /// <returns>This instance.</returns>
    public TinyMceOptions WithLicenseKey(string licenseKey)
    {
        LicenseKey = licenseKey;
        IncludeLicense = true;
        return this;
    }

    /// <summary>Sets <see cref="Promotion"/> and returns this instance. The value is sent only when <see cref="IncludeLicense"/> is set.</summary>
    /// <param name="show"><see langword="false"/> hides the promotion. The default argument is <see langword="true"/>.</param>
    /// <returns>This instance.</returns>
    public TinyMceOptions ShowPromotion(bool show = true)
    {
        Promotion = show;
        return this;
    }

    /// <summary>Sets <see cref="RelativeUrls"/> and returns this instance.</summary>
    /// <param name="relative"><see langword="false"/> keeps root-relative URLs. The default argument is <see langword="true"/>.</param>
    /// <returns>This instance.</returns>
    public TinyMceOptions UseRelativeUrls(bool relative = true)
    {
        RelativeUrls = relative;
        return this;
    }

    /// <summary>Sets <see cref="FilePickerTypes"/> and returns this instance.</summary>
    /// <param name="types">Picker types separated by spaces, such as <c>image</c>.</param>
    /// <returns>This instance.</returns>
    public TinyMceOptions WithFilePickerTypes(string types)
    {
        FilePickerTypes = types;
        return this;
    }

    /// <summary>Replaces the plugin list and returns this instance.</summary>
    /// <param name="plugins">The plugin names.</param>
    /// <returns>This instance.</returns>
    public TinyMceOptions WithPlugins(params string[] plugins)
    {
        Plugins = [.. plugins];
        return this;
    }

    /// <summary>Adds plugins that are not already listed and returns this instance. The comparison ignores case.</summary>
    /// <param name="plugins">The plugin names to add.</param>
    /// <returns>This instance.</returns>
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
    /// Sets <see cref="Toolbar"/>, clears any rows set with <see cref="WithToolbars"/>, and returns this instance.
    /// </summary>
    /// <param name="toolbar">The toolbar.</param>
    /// <returns>The same options instance.</returns>
    public TinyMceOptions WithToolbar(string toolbar)
    {
        Toolbar = toolbar;
        Toolbars = null;
        return this;
    }

    /// <summary>Shows one toolbar row for each string. Replaces <see cref="WithToolbar"/>.</summary>
    /// <param name="toolbars">The toolbar rows, from top to bottom.</param>
    /// <returns>The same options instance.</returns>
    public TinyMceOptions WithToolbars(params string[] toolbars)
    {
        ArgumentNullException.ThrowIfNull(toolbars);
        Toolbars = [.. toolbars];
        return this;
    }

    /// <summary>
    /// Stores one TinyMCE init value and returns this instance.
    /// A structured setting written later, such as the toolbar or the file picker, replaces the same name.
    /// </summary>
    /// <param name="name">The init property name.</param>
    /// <param name="value">The value passed through to TinyMCE.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or white space.</exception>
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
        if (MinHeight is int minHeight)
        {
            init["min_height"] = minHeight;
        }

        if (MaxHeight is int maxHeight)
        {
            init["max_height"] = maxHeight;
        }

        init["menubar"] = Menubar;
        init["plugins"] = string.Join(' ', Plugins);
        if (Toolbars is { Count: > 0 } toolbars)
        {
            init["toolbar"] = toolbars;
        }
        else
        {
            init["toolbar"] = Toolbar;
        }
        init["relative_urls"] = RelativeUrls;
        init["remove_script_host"] = RemoveScriptHost;
        init["convert_urls"] = ConvertUrls;
        init["file_picker_types"] = FilePickerTypes;

        return init;
    }
}