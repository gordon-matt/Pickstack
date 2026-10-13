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
        options.UploadDirectory = _storage.RequestPath;
        options.CurrentPath = _storage.PhysicalPhpPath;
        options.ThumbsBasePath = _storage.ThumbnailPhysicalPhpPath;
        options.ThumbsUploadDir = _storage.ThumbnailRequestPath;
        if (options.MaxSizeUpload <= 0)
        {
            options.MaxSizeUpload = 32;
        }

        _configure?.Invoke(options);
    }
}