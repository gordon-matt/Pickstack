using Microsoft.Extensions.Options;
using Pickstack.FileManager;

namespace Pickstack.FileManager.ResponsiveFileManager;

internal sealed class ResponsiveFileManagerConfigurator : IConfigureOptions<global::ResponsiveFileManager.ResponsiveFileManagerOptions>
{
    private readonly IFileStorage _storage;
    private readonly Action<global::ResponsiveFileManager.ResponsiveFileManagerOptions>? _configure;

    public ResponsiveFileManagerConfigurator(IFileStorage storage, IOptions<ResponsiveFileManagerOptions> picker)
    {
        _storage = storage;
        _configure = picker.Value.ConfigureServer;
    }

    public void Configure(global::ResponsiveFileManager.ResponsiveFileManagerOptions options)
    {
        // upload.php joins these onto the /filemanager/ script directory, and the dialog
        // uses the same thumbnail path as the image address. Absolute filesystem paths
        // break both. ../media/ from /filemanager/dialog.php is wwwroot/media.
        options.UploadDirectory = _storage.RequestPath;
        options.CurrentPath = ToFileManagerRelativePath(_storage.RequestPath);
        options.ThumbsBasePath = ToFileManagerRelativePath(_storage.ThumbnailRequestPath);
        options.ThumbsUploadDir = _storage.ThumbnailRequestPath;
        if (options.MaxSizeUpload <= 0)
        {
            options.MaxSizeUpload = 32;
        }

        _configure?.Invoke(options);
    }

    internal static string ToFileManagerRelativePath(string requestPath)
    {
        string relative = requestPath.Trim('/');
        string[] segments = relative.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0 || segments.Any(FileRoots.IsUnsafe))
        {
            throw new InvalidOperationException($"'{requestPath}' is not a valid Responsive File Manager folder.");
        }

        return "../" + string.Join('/', segments) + "/";
    }
}