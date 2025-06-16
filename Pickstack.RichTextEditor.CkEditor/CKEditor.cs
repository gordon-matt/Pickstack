using Pickstack.FileManager;

namespace Pickstack.RichTextEditor.CkEditor;

public class CKEditor : IRichTextEditor
{
    public string Name => "CKEditor";

    public string GetEditorScript(EditorConfig config) => "/lib/ckeditor/ckeditor.js";

    public object GetEditorConfiguration(EditorConfig config, IFileManager fileManager)
    {
        object fileManagerConfig = fileManager.GetJsonConfig(new FileManagerConfig());

        var defaultConfig = new Dictionary<string, object>
        {
            ["height"] = config.Height,
            ["width"] = config.Width,
            ["toolbar"] = GetToolbarConfig(config.Toolbar),
            ["filebrowserBrowseUrl"] = GetFileBrowserUrl(fileManager, fileManagerConfig, "file"),
            ["filebrowserImageBrowseUrl"] = GetFileBrowserUrl(fileManager, fileManagerConfig, "image"),
            ["filebrowserUploadUrl"] = GetUploadUrl(fileManager),
            ["filebrowserImageUploadUrl"] = GetUploadUrl(fileManager)
        };

        // Merge custom options
        foreach (var option in config.CustomOptions)
        {
            defaultConfig[option.Key] = option.Value;
        }

        return defaultConfig;
    }

    private object GetToolbarConfig(string toolbar) => !string.IsNullOrEmpty(toolbar)
        ? toolbar
        : (new object[] // Explicit object array
        {
            new { name = "document", groups = new[] { "mode", "document", "doctools" }, items = new[] { "Source" } },
            new { name = "clipboard", groups = new[] { "clipboard", "undo" }, items = new[] { "Cut", "Copy", "Paste", "Undo", "Redo" } },
            new { name = "editing", groups = new[] { "find", "selection", "spellchecker" }, items = new[] { "Find", "Replace" } },
            "/", // Can keep as string
            new { name = "basicstyles", groups = new[] { "basicstyles", "cleanup" }, items = new[] { "Bold", "Italic", "Underline", "Strike" } },
            new { name = "paragraph", groups = new[] { "list", "indent", "blocks", "align", "bidi" }, items = new[] { "NumberedList", "BulletedList", "JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock" } },
            new { name = "links", items = new[] { "Link", "Unlink" } },
            new { name = "insert", items = new[] { "Image", "Table", "HorizontalRule" } }
        });

    private string GetFileBrowserUrl(IFileManager fileManager, object fileManagerConfig, string type) => fileManager.Name.ToLower() switch
    {
        "elfinder" => ((dynamic)fileManagerConfig).url + $"?type={type}",
        "responsivefilemanager" => ((dynamic)fileManagerConfig).url + $"&type={type}",
        _ => "/filemanager/browse"
    };

    private string GetUploadUrl(IFileManager fileManager) => fileManager.Name.ToLower() switch
    {
        "elfinder" => "/filemanager/elfinder/connector?cmd=upload",
        "responsivefilemanager" => "/filemanager/upload.php",
        _ => "/filemanager/upload"
    };
}