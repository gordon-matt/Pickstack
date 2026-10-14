namespace Pickstack.FileManager;

public interface IFileStorage
{
    string PhysicalPath { get; }

    string RequestPath { get; }

    string ThumbnailPhysicalPath { get; }

    string ThumbnailRequestPath { get; }

    void EnsureCreated();

    /// <summary>Creates <paramref name="root"/> under the upload folder. A blank root ensures the upload folder itself.</summary>
    void EnsureRoot(string? root);

    string ResolvePhysicalPath(string? root);

    string ResolveRequestPath(string? root);
}