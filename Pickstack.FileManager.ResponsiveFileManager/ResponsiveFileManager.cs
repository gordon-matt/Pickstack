namespace Pickstack.FileManager.ResponsiveFileManager;

public class ResponsiveFileManager : IFileManager
{
    public string Name => "Responsive File Manager";

    public string GetMarkup(FileManagerConfig config) =>
$@"<iframe src=""/filemanager/dialog.php?rootFolder=&type=0&ignore_last_position=1""
frameborder=""0""
style=""overflow:hidden;min-height:600px; height:100%;width:100%""
height=""100%""
width=""100%"">";

    public string GetJsonConfig(FileManagerConfig config) => null;
}