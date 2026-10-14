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

        string folder = NormalizeFolder(_options.WebRootFolder, "media");
        string thumbnailFolder = NormalizeFolder(_options.ThumbnailWebRootFolder, "media-thumbs");
        if (Contains(folder, thumbnailFolder) || Contains(thumbnailFolder, folder))
        {
            throw new InvalidOperationException("The thumbnail folder must be outside the upload folder.");
        }
    }

    public string PhysicalPath => Path.Combine(_webRoot, NormalizeFolder(_options.WebRootFolder, "media"));

    public string RequestPath => NormalizeRequestPath(_options.RequestPath, "/media/");

    public string ThumbnailPhysicalPath => Path.Combine(_webRoot, NormalizeFolder(_options.ThumbnailWebRootFolder, "media-thumbs"));

    public string ThumbnailRequestPath => NormalizeRequestPath(_options.ThumbnailRequestPath, "/media-thumbs/");

    public void EnsureCreated()
    {
        Directory.CreateDirectory(PhysicalPath);
        Directory.CreateDirectory(ThumbnailPhysicalPath);
    }

    public void EnsureRoot(string? root) => Directory.CreateDirectory(ResolvePhysicalPath(root));

    public string ResolvePhysicalPath(string? root)
    {
        string? relative = FileRoots.Normalize(root);
        if (relative is null)
        {
            return PhysicalPath;
        }

        string full = Path.GetFullPath(Path.Combine(PhysicalPath, relative.Replace('/', Path.DirectorySeparatorChar)));
        string storage = Path.GetFullPath(PhysicalPath);
        string prefix = storage.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (!full.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The file root must stay inside the upload folder.");
        }

        return full;
    }

    public string ResolveRequestPath(string? root)
    {
        string? relative = FileRoots.Normalize(root);
        return relative is null ? RequestPath : RequestPath + relative + "/";
    }

    internal static string NormalizeFolder(string? value, string fallback)
    {
        string path = string.IsNullOrWhiteSpace(value) ? fallback : value;
        string[] segments = path.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (segments.Length == 0 || segments.Any(FileRoots.IsUnsafe))
        {
            throw new InvalidOperationException($"'{value}' must be a folder path under wwwroot.");
        }

        return Path.Combine(segments);
    }

    private static bool Contains(string parent, string child)
    {
        string parentPath = parent.Replace('\\', '/').Trim('/') + "/";
        string childPath = child.Replace('\\', '/').Trim('/') + "/";
        return childPath.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase);
    }

    internal static string NormalizeRequestPath(string? value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            value = fallback;
        }

        string path = value.Replace('\\', '/').Trim();
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
}