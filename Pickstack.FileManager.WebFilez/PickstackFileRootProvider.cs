using Microsoft.Extensions.Options;
using WebFilez.Services;
using WebFilez.Services.Access;
using WebFilez.Services.Storage;

namespace Pickstack.FileManager.WebFilez;

/// <summary>
/// Exposes Pickstack's shared upload folder as a WebFilez root, then any extra roots from configuration.
/// </summary>
internal sealed class PickstackFileRootProvider : IFileRootProvider
{
    private readonly IFileStorage _storage;
    private readonly IOptions<WebFilezFileManagerOptions> _picker;
    private readonly IOptions<WebFilezOptions> _webFilez;

    public PickstackFileRootProvider(
        IFileStorage storage,
        IOptions<WebFilezFileManagerOptions> picker,
        IOptions<WebFilezOptions> webFilez)
    {
        _storage = storage;
        _picker = picker;
        _webFilez = webFilez;
    }

    public Task<IReadOnlyList<FileRoot>> GetRootsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _storage.EnsureCreated();

        var options = _picker.Value;
        var roots = new List<FileRoot>
        {
            new()
            {
                Id = options.RootId,
                Name = options.RootName,
                Provider = FileSystemStorageProvider.ProviderKey,
                Path = _storage.PhysicalPath,
                PublicUrl = PublicUrl(_storage.RequestPath)
            }
        };

        foreach (var extra in _webFilez.Value.Roots)
        {
            if (string.IsNullOrWhiteSpace(extra.Id))
            {
                continue;
            }

            if (roots.Any(root => string.Equals(root.Id, extra.Id, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            roots.Add(extra);
        }

        return Task.FromResult<IReadOnlyList<FileRoot>>(roots);
    }

    private static string PublicUrl(string requestPath)
    {
        string url = requestPath.TrimEnd('/');
        return url.Length == 0 ? "/" : url;
    }
}
