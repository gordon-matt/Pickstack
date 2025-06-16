using Pickstack.FileManager;

namespace Pickstack.RichTextEditor;

public interface IRichTextEditor
{
    string Name { get; }

    string GetEditorScript(EditorConfig config);

    object GetEditorConfiguration(EditorConfig config, IFileManager fileManager);
}