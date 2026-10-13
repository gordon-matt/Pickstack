namespace Pickstack.FileManager;

/// <summary>
/// Where every file manager reads and writes. Adapters share this location so a swap
/// does not orphan files that were uploaded with the previous manager.
/// </summary>
public sealed class FileStorageOptions
{
    public const string SectionName = "Pickstack:Storage";

    /// <summary>Public URL prefix. A leading and trailing slash are added when missing.</summary>
    public string RequestPath { get; set; } = "/media/";

    /// <summary>Single folder name under wwwroot.</summary>
    public string WebRootFolder { get; set; } = "media";

    public string ThumbnailRequestPath { get; set; } = "/media-thumbs/";

    /// <summary>Single folder name under wwwroot, outside <see cref="WebRootFolder"/>.</summary>
    public string ThumbnailWebRootFolder { get; set; } = "media-thumbs";
}
