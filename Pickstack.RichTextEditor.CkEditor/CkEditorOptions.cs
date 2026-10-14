namespace Pickstack.RichTextEditor.CkEditor;

/// <summary>
/// Site-wide CKEditor 4 settings. An empty toolbar leaves CKEditor's own toolbar in place.
/// The file browser URLs stay pointed at the registered Pickstack file manager.
/// </summary>
public sealed class CkEditorOptions
{
    public string ScriptUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/ckeditor/4.17.1/ckeditor.js";

    public int Height { get; set; } = 420;

    public int BrowseWindowWidth { get; set; } = 980;

    public int BrowseWindowHeight { get; set; } = 680;

    public List<string[]> Toolbar { get; set; } = [];

    public List<string> RemovePlugins { get; set; } = [];

    public List<string> ExtraPlugins { get; set; } = [];

    /// <summary>Extra CKEditor config values. Structured settings and the file browser URLs are applied after these.</summary>
    public Dictionary<string, object?> CustomOptions { get; } = [];

    public CkEditorOptions WithScript(string url)
    {
        ScriptUrl = url;
        return this;
    }

    public CkEditorOptions WithHeight(int pixels)
    {
        Height = pixels;
        return this;
    }

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

    public CkEditorOptions AddToolbarGroup(params string[] items)
    {
        Toolbar.Add(items);
        return this;
    }

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

    public CkEditorOptions WithExtraPlugins(params string[] plugins)
    {
        ExtraPlugins = [.. plugins];
        return this;
    }

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