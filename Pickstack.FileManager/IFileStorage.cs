namespace Pickstack.FileManager;

/// <summary>
/// Defines the contract for the file storage.
/// </summary>
public interface IFileStorage
{
    /// <summary>
    /// Gets the physical path.
    /// </summary>
    string PhysicalPath { get; }

    /// <summary>
    /// Gets the request path.
    /// </summary>
    string RequestPath { get; }

    /// <summary>
    /// Gets the thumbnail physical path.
    /// </summary>
    string ThumbnailPhysicalPath { get; }

    /// <summary>
    /// Gets the thumbnail request path.
    /// </summary>
    string ThumbnailRequestPath { get; }

    /// <summary>
    /// Ensures the created.
    /// </summary>
    void EnsureCreated();

    /// <summary>Creates <paramref name="root"/> under the upload folder. A blank root ensures the upload folder itself.</summary>
    void EnsureRoot(string? root);

    /// <summary>
    /// Resolves the physical path.
    /// </summary>
    /// <param name="root">The root.</param>
    /// <returns>The physical path.</returns>
    string ResolvePhysicalPath(string? root);

    /// <summary>
    /// Resolves the request path.
    /// </summary>
    /// <param name="root">The root.</param>
    /// <returns>The request path.</returns>
    string ResolveRequestPath(string? root);
}