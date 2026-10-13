namespace Pickstack.FileManager;

public interface IFileStorage
{
    string PhysicalPath { get; }

    string RequestPath { get; }

    string ThumbnailPhysicalPath { get; }

    string ThumbnailRequestPath { get; }

    void EnsureCreated();
}