namespace Pickstack.RichTextEditor.CkEditor;

/// <summary>
/// Site-wide CKEditor 4 settings. An empty toolbar leaves CKEditor's own toolbar in place.
/// The file browser URLs stay pointed at the registered Pickstack file manager.
/// </summary>
public sealed class CkEditorOptions
{
    /// <summary>
    /// Gets or sets the script URL.
    /// </summary>
    public string ScriptUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/ckeditor/4.17.1/ckeditor.js";

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    public int Height { get; set; } = 420;

    /// <summary>
    /// Gets or sets the browse window width.
    /// </summary>
    public int BrowseWindowWidth { get; set; } = 980;

    /// <summary>
    /// Gets or sets the browse window height.
    /// </summary>
    public int BrowseWindowHeight { get; set; } = 680;

    /// <summary>
    /// Gets or sets the toolbar.
    /// </summary>
    public List<string[]> Toolbar { get; set; } = [];

    /// <summary>
    /// Gets or sets the remove plugins.
    /// </summary>
    public List<string> RemovePlugins { get; set; } = [];

    /// <summary>
    /// Gets or sets the extra plugins.
    /// </summary>
    public List<string> ExtraPlugins { get; set; } = [];

    /// <summary>Extra CKEditor config values. Structured settings and the file browser URLs are applied after these.</summary>
    public Dictionary<string, object?> CustomOptions { get; } = [];

    /// <summary>
    /// Returns a copy with the script.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The script.</returns>
    public CkEditorOptions WithScript(string url)
    {
        ScriptUrl = url;
        return this;
    }

    /// <summary>
    /// Returns a copy with the height.
    /// </summary>
    /// <param name="pixels">The pixels.</param>
    /// <returns>The height.</returns>
    public CkEditorOptions WithHeight(int pixels)
    {
        Height = pixels;
        return this;
    }

    /// <summary>
    /// Returns a copy with the browse window.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns>The browse window.</returns>
    public CkEditorOptions WithBrowseWindow(int width, int height)
    {
        BrowseWindowWidth = width;
        BrowseWindowHeight = height;
        return this;
    }

    /// <summary>Replaces the toolbar with these groups. Each group is one cluster of buttons.</summary>
    public CkEditorOptions WithToolbar(params string[][] groups)
    {
        Toolbar = [.. groups];
        return this;
    }

    /// <summary>
    /// Adds the toolbar group.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns>The toolbar group.</returns>
    public CkEditorOptions AddToolbarGroup(params string[] items)
    {
        Toolbar.Add(items);
        return this;
    }

    /// <summary>
    /// Gets the without plugins.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>The without plugins.</returns>
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

    /// <summary>
    /// Returns a copy with the extra plugins.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>The extra plugins.</returns>
    public CkEditorOptions WithExtraPlugins(params string[] plugins)
    {
        ExtraPlugins = [.. plugins];
        return this;
    }

    /// <summary>
    /// Adds the extra plugins.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>The extra plugins.</returns>
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
    /// Sets the value.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns>The ck editor options.</returns>
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