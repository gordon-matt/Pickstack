namespace Pickstack.FileManager;

public interface IFileStorage
{
    string PhysicalPath { get; }

    string RequestPath { get; }

    /// <summary>Absolute filesystem path with forward slashes, for PHP-based managers.</summary>
    string PhysicalPhpPath { get; }

    string ThumbnailPhysicalPath { get; }

    string ThumbnailRequestPath { get; }

    string ThumbnailPhysicalPhpPath { get; }

    void EnsureCreated();
}
