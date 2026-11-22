namespace Pickstack.FileManager;

/// <summary>
/// Where every file manager reads and writes. Adapters share this location so a swap
/// does not orphan files that were uploaded with the previous manager.
/// </summary>
public sealed class FileStorageOptions
{
    /// <summary>The configuration section <c>AddPickstack</c> binds. The value is <c>Pickstack:Storage</c>.</summary>
    public const string SectionName = "Pickstack:Storage";

    /// <summary>Public URL prefix. A leading and trailing slash are added when missing. A value containing <c>..</c> is rejected. The default is <c>/media/</c>.</summary>
    public string RequestPath { get; set; } = "/media/";

    /// <summary>Folder path under wwwroot. Nested folders such as <c>Media/Uploads</c> are allowed.</summary>
    public string WebRootFolder { get; set; } = "media";

    /// <summary>Public URL prefix for thumbnails. A leading and trailing slash are added when missing. A value containing <c>..</c> is rejected. The default is <c>/media-thumbs/</c>.</summary>
    public string ThumbnailRequestPath { get; set; } = "/media-thumbs/";

    /// <summary>Folder path under wwwroot, outside <see cref="WebRootFolder"/>.</summary>
    public string ThumbnailWebRootFolder { get; set; } = "media-thumbs";
}