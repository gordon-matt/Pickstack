namespace Pickstack.RichTextEditor;

public class EditorConfig
{
    public string Theme { get; set; } = "modern";

    public List<string> Plugins { get; set; } = [];

    public string Toolbar { get; set; }

    public Dictionary<string, object> CustomOptions { get; set; } = [];

    public int Width { get; set; } = 800;

    public int Height { get; set; } = 400;
}