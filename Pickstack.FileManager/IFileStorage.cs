namespace Pickstack.FileManager;

/// <summary>
/// The shared upload folder. Every file manager reads and writes here, so a swap keeps the files.
/// The folder is a physical directory under the web root.
/// </summary>
public interface IFileStorage
{
    /// <summary>Absolute path of the upload folder. A folder option that is not a safe path under the web root throws when this is read.</summary>
    string PhysicalPath { get; }

    /// <summary>Public URL prefix of the upload folder, with a leading and trailing slash. A value containing <c>..</c> throws when this is read.</summary>
    string RequestPath { get; }

    /// <summary>Absolute path of the thumbnail folder. It is outside the upload folder.</summary>
    string ThumbnailPhysicalPath { get; }

    /// <summary>Public URL prefix of the thumbnail folder, with a leading and trailing slash.</summary>
    string ThumbnailRequestPath { get; }

    /// <summary>Creates the upload folder and the thumbnail folder when they are missing.</summary>
    void EnsureCreated();

    /// <summary>Creates <paramref name="root"/> under the upload folder. A blank root creates the upload folder itself.</summary>
    /// <param name="root">A relative folder such as <c>42/7</c>, or <see langword="null"/> for the upload folder.</param>
    void EnsureRoot(string? root);

    /// <summary>The absolute path of <paramref name="root"/> under the upload folder.</summary>
    /// <param name="root">A relative folder, or <see langword="null"/> for the upload folder.</param>
    /// <returns>The absolute path.</returns>
    /// <exception cref="InvalidOperationException"><paramref name="root"/> is not a valid file root, or the path leaves the upload folder.</exception>
    string ResolvePhysicalPath(string? root);

    /// <summary>The public URL of <paramref name="root"/>, with a trailing slash.</summary>
    /// <param name="root">A relative folder, or <see langword="null"/> for the upload folder.</param>
    /// <returns>The URL prefix.</returns>
    /// <exception cref="InvalidOperationException"><paramref name="root"/> is not a valid file root.</exception>
    string ResolveRequestPath(string? root);
}