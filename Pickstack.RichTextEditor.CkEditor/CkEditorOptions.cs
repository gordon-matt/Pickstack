namespace Pickstack.RichTextEditor.CkEditor;

/// <summary>
/// Site-wide CKEditor 4 settings. An empty toolbar leaves CKEditor's own toolbar in place.
/// When a file manager is registered, the provider script points Browse Server at it.
/// An editor instance can set <c>file-manager="false"</c> and keep CKEditor's own dialogs.
/// </summary>
public sealed class CkEditorOptions
{
    /// <summary>Address of <c>ckeditor.js</c>. The default is CKEditor 4.17.1 on cdnjs.</summary>
    public string ScriptUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/ckeditor/4.17.1/ckeditor.js";

    /// <summary>Editor height in pixels. The default is 420. A field can override it.</summary>
    public int Height { get; set; } = 420;

    /// <summary>
    /// Minimum height of the resize handle, in pixels. Leave unset to keep CKEditor's own default.
    /// </summary>
    public int? MinHeight { get; set; }

    /// <summary>
    /// Maximum height of the resize handle, in pixels. Leave unset for no maximum.
    /// </summary>
    public int? MaxHeight { get; set; }

    /// <summary>Width of the Browse Server window, in pixels. The default is 980.</summary>
    public int BrowseWindowWidth { get; set; } = 980;

    /// <summary>Height of the Browse Server window, in pixels. The default is 680.</summary>
    public int BrowseWindowHeight { get; set; } = 680;

    /// <summary>Toolbar groups. Each array is one cluster of buttons. Empty leaves CKEditor's own toolbar.</summary>
    public List<string[]> Toolbar { get; set; } = [];

    /// <summary>Plugins removed from CKEditor. Empty removes none.</summary>
    public List<string> RemovePlugins { get; set; } = [];

    /// <summary>Plugins added to CKEditor. Empty adds none.</summary>
    public List<string> ExtraPlugins { get; set; } = [];

    /// <summary>Extra CKEditor config values. Structured settings and the file browser URLs are applied after these.</summary>
    public Dictionary<string, object?> CustomOptions { get; } = [];

    /// <summary>Sets <see cref="ScriptUrl"/> and returns this instance.</summary>
    /// <param name="url">Address of <c>ckeditor.js</c>.</param>
    /// <returns>This instance.</returns>
    public CkEditorOptions WithScript(string url)
    {
        ScriptUrl = url;
        return this;
    }

    /// <summary>Sets <see cref="Height"/> and returns this instance.</summary>
    /// <param name="pixels">The height, in pixels.</param>
    /// <returns>This instance.</returns>
    public CkEditorOptions WithHeight(int pixels)
    {
        Height = pixels;
        return this;
    }

    /// <summary>Sets the minimum editor height, in pixels.</summary>
    /// <param name="pixels">The pixels.</param>
    /// <returns>The same options instance.</returns>
    public CkEditorOptions WithMinHeight(int pixels)
    {
        MinHeight = pixels;
        return this;
    }

    /// <summary>Sets the maximum editor height, in pixels.</summary>
    /// <param name="pixels">The pixels.</param>
    /// <returns>The same options instance.</returns>
    public CkEditorOptions WithMaxHeight(int pixels)
    {
        MaxHeight = pixels;
        return this;
    }

    /// <summary>Sets the Browse Server window size and returns this instance.</summary>
    /// <param name="width">The width, in pixels.</param>
    /// <param name="height">The height, in pixels.</param>
    /// <returns>This instance.</returns>
    public CkEditorOptions WithBrowseWindow(int width, int height)
    {
        BrowseWindowWidth = width;
        BrowseWindowHeight = height;
        return this;
    }

    /// <summary>Replaces the toolbar with these groups and returns this instance. Each group is one cluster of buttons. An empty list leaves CKEditor's own toolbar.</summary>
    /// <param name="groups">The button groups, in order.</param>
    /// <returns>This instance.</returns>
    public CkEditorOptions WithToolbar(params string[][] groups)
    {
        Toolbar = [.. groups];
        return this;
    }

    /// <summary>Appends one toolbar group and returns this instance. Once any group is set, CKEditor's own toolbar is no longer used.</summary>
    /// <param name="items">The button names in the new group.</param>
    /// <returns>This instance.</returns>
    public CkEditorOptions AddToolbarGroup(params string[] items)
    {
        Toolbar.Add(items);
        return this;
    }

    /// <summary>Adds plugins to <see cref="RemovePlugins"/> and returns this instance. A plugin already listed is left once. The comparison ignores case.</summary>
    /// <param name="plugins">The plugin names to remove.</param>
    /// <returns>This instance.</returns>
    public CkEditorOptions WithoutPlugins(params string[] plugins)
    {
        foreach (string plugin in plugins)
        {
            if (!RemovePlugins.Contains(plugin, StringComparer.OrdinalIgnoreCase))
            {
                RemovePlugins.Add(plugin);
            }
        }

        return this;
    }

    /// <summary>Replaces <see cref="ExtraPlugins"/> and returns this instance.</summary>
    /// <param name="plugins">The plugin names.</param>
    /// <returns>This instance.</returns>
    public CkEditorOptions WithExtraPlugins(params string[] plugins)
    {
        ExtraPlugins = [.. plugins];
        return this;
    }

    /// <summary>Adds plugins that are not already in <see cref="ExtraPlugins"/> and returns this instance. The comparison ignores case.</summary>
    /// <param name="plugins">The plugin names to add.</param>
    /// <returns>This instance.</returns>
    public CkEditorOptions AddExtraPlugins(params string[] plugins)
    {
        foreach (string plugin in plugins)
        {
            if (!ExtraPlugins.Contains(plugin, StringComparer.OrdinalIgnoreCase))
            {
                ExtraPlugins.Add(plugin);
            }
        }

        return this;
    }

    /// <summary>
    /// Stores one CKEditor config value and returns this instance.
    /// A structured setting written later, and the file browser URLs, replace the same name.
    /// </summary>
    /// <param name="name">The config property name.</param>
    /// <param name="value">The value passed through to CKEditor.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or white space.</exception>
    public CkEditorOptions Set(string name, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        CustomOptions[name] = value;
        return this;
    }

    internal Dictionary<string, object?> CreateConfig()
    {
        var config = new Dictionary<string, object?>(CustomOptions)
        {
            ["height"] = Height,
            ["filebrowserWindowWidth"] = BrowseWindowWidth,
            ["filebrowserWindowHeight"] = BrowseWindowHeight
        };

        if (MinHeight is int minHeight)
        {
            config["resize_minHeight"] = minHeight;
        }

        if (MaxHeight is int maxHeight)
        {
            config["resize_maxHeight"] = maxHeight;
        }

        if (Toolbar.Count > 0)
        {
            config["toolbar"] = Toolbar;
        }

        if (RemovePlugins.Count > 0)
        {
            config["removePlugins"] = string.Join(',', RemovePlugins);
        }

        if (ExtraPlugins.Count > 0)
        {
            config["extraPlugins"] = string.Join(',', ExtraPlugins);
        }

        return config;
    }
}