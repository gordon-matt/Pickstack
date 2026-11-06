namespace Pickstack.FileManager;

/// <summary>
/// Where every file manager reads and writes. Adapters share this location so a swap
/// does not orphan files that were uploaded with the previous manager.
/// </summary>
public sealed class FileStorageOptions
{
    /// <summary>
    /// The section name constant.
    /// </summary>
    public const string SectionName = "Pickstack:Storage";

    /// <summary>Public URL prefix. A leading and trailing slash are added when missing.</summary>
    public string RequestPath { get; set; } = "/media/";

    /// <summary>Folder path under wwwroot. Nested folders such as <c>Media/Uploads</c> are allowed.</summary>
    public string WebRootFolder { get; set; } = "media";

    /// <summary>
    /// Gets or sets the thumbnail request path.
    /// </summary>
    public string ThumbnailRequestPath { get; set; } = "/media-thumbs/";

    /// <summary>Folder path under wwwroot, outside <see cref="WebRootFolder"/>.</summary>
    public string ThumbnailWebRootFolder { get; set; } = "media-thumbs";
}