namespace Pickstack.FileManager;

public interface IFileManager
{
    string Name { get; }

    string GetMarkup(FileManagerConfig config);

    string GetJsonConfig(FileManagerConfig config);
}