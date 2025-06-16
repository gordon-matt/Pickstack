using Pickstack.FileManager;

namespace Pickstack.RichTextEditor.TinyMce;

public class TinyMce : IRichTextEditor
{
    public string Name => "TinyMCE";

    public string GetEditorScript(EditorConfig config) => "/lib/tinymce/tinymce.min.js";

    public object GetEditorConfiguration(EditorConfig config, IFileManager fileManager)
    {
        object fileManagerConfig = fileManager.GetJsonConfig(new FileManagerConfig());

        var defaultConfig = new Dictionary<string, object>
        {
            ["height"] = config.Height,
            ["width"] = config.Width,
            ["plugins"] = string.Join(" ", config.Plugins.Any() ? config.Plugins : new[] { "image", "link", "media", "table" }),
            ["toolbar"] = config.Toolbar ?? "undo redo | bold italic | alignleft aligncenter alignright | bullist numlist | link image",
            ["file_picker_callback"] = GetFilePickerCallback(fileManager, fileManagerConfig),
            ["relative_urls"] = false,
            ["remove_script_host"] = false
        };

        // Merge custom options
        foreach (var option in config.CustomOptions)
        {
            defaultConfig[option.Key] = option.Value;
        }

        return defaultConfig;
    }

    private string GetFilePickerCallback(IFileManager fileManager, object fileManagerConfig) => fileManager.Name.ToLower() switch
    {
        "elfinder" => @"
                function(callback, value, meta) {
                    var elfinder = $('<div>').elfinder({
                        url: '" + ((dynamic)fileManagerConfig).url + @"',
                        getFileCallback: function(files) {
                            callback(files.url);
                        }
                    }).elfinder('instance');
                }",

        "responsivefilemanager" => @"
                function(callback, value, meta) {
                    var width = " + ((dynamic)fileManagerConfig).width + @";
                    var height = " + ((dynamic)fileManagerConfig).height + @";
                    var url = '" + ((dynamic)fileManagerConfig).url + @"';

                    tinymce.activeEditor.windowManager.openUrl({
                        url: url,
                        title: 'File Manager',
                        width: width,
                        height: height,
                        resizable: true,
                        maximizable: true,
                        onMessage: function(api, message) {
                            if (message.mceAction === 'fileSelected') {
                                callback(message.url);
                                api.close();
                            }
                        }
                    });
                }",

        _ => "function(callback, value, meta) { console.log('File picker not configured'); }"
    };
}