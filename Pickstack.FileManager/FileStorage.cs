using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Pickstack.FileManager;

public sealed class FileStorage : IFileStorage
{
    private readonly string _webRoot;
    private readonly FileStorageOptions _options;

    public FileStorage(IWebHostEnvironment environment, IOptions<FileStorageOptions> options)
    {
        if (string.IsNullOrEmpty(environment.WebRootPath))
        {
            throw new InvalidOperationException("Pickstack requires a wwwroot directory.");
        }

        _webRoot = environment.WebRootPath;
        _options = options.Value;

        var folder = NormalizeFolder(_options.WebRootFolder, "media");
        var thumbnailFolder = NormalizeFolder(_options.ThumbnailWebRootFolder, "media-thumbs");
        if (string.Equals(folder, thumbnailFolder, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The thumbnail folder must be outside the upload folder.");
        }
    }

    public string PhysicalPath => Path.Combine(_webRoot, NormalizeFolder(_options.WebRootFolder, "media"));

    public string RequestPath => NormalizeRequestPath(_options.RequestPath, "/media/");

    public string PhysicalPhpPath => ToPhpPath(PhysicalPath);

    public string ThumbnailPhysicalPath => Path.Combine(_webRoot, NormalizeFolder(_options.ThumbnailWebRootFolder, "media-thumbs"));

    public string ThumbnailRequestPath => NormalizeRequestPath(_options.ThumbnailRequestPath, "/media-thumbs/");

    public string ThumbnailPhysicalPhpPath => ToPhpPath(ThumbnailPhysicalPath);

    public void EnsureCreated()
    {
        Directory.CreateDirectory(PhysicalPath);
        Directory.CreateDirectory(ThumbnailPhysicalPath);
    }

    internal static string NormalizeFolder(string? value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        var folder = value.Trim();
        if (folder is "." or ".."
            || folder.Contains('/', StringComparison.Ordinal)
            || folder.Contains('\\', StringComparison.Ordinal)
            || folder.Contains(':', StringComparison.Ordinal)
            || Path.IsPathRooted(folder))
        {
            throw new InvalidOperationException($"'{value}' must be a single folder name under wwwroot.");
        }

        return folder;
    }

    internal static string NormalizeRequestPath(string? value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            value = fallback;
        }

        var path = value.Replace('\\', '/').Trim();
        if (path.Contains("..", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Request path '{value}' is not allowed.");
        }

        if (!path.StartsWith('/'))
        {
            path = "/" + path;
        }

        if (!path.EndsWith('/'))
        {
            path += "/";
        }

        return path;
    }

    internal static string ToPhpPath(string physicalPath)
    {
        var full = Path.GetFullPath(physicalPath).Replace('\\', '/');
        return full.EndsWith('/') ? full : full + "/";
    }
}
