using elFinder.NetCore;
using elFinder.NetCore.Drivers.FileSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Pickstack.FileManager.elFinder;

/// <summary>
/// Represents the el finder connector controller.
/// </summary>
[Route(ElFinderRoutes.Prefix)]
public sealed class ElFinderConnectorController : Controller
{
    private readonly IFileStorage _storage;
    private readonly ILogger<ElFinderConnectorController> _logger;

    /// <summary>
    /// Initializes a new instance of the ElFinderConnectorController class.
    /// </summary>
    /// <param name="storage">The storage.</param>
    /// <param name="logger">The logger.</param>
    public ElFinderConnectorController(IFileStorage storage, ILogger<ElFinderConnectorController> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    /// <summary>
    /// Gets the connector.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the i action result.</returns>
    [Route("connector")]
    public async Task<IActionResult> Connector()
    {
        try
        {
            return await CreateConnector(ReadRoot()).ProcessAsync(Request);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "elFinder could not process a connector request.");
            return Json(new { error = "Unable to process the file manager request." });
        }
    }

    /// <summary>
    /// Gets the thumbnail.
    /// </summary>
    /// <param name="hash">The hash.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the i action result.</returns>
    [Route("thumb/{*hash}")]
    public async Task<IActionResult> Thumbnail(string hash)
    {
        try
        {
            (string? root, string fileHash) = SplitThumbnail(hash);
            return await CreateConnector(root).GetThumbnailAsync(Request, Response, fileHash);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "elFinder could not create a thumbnail.");
            return Json(new { error = "Unable to process the file manager request." });
        }
    }

    private Connector CreateConnector(string? root)
    {
        _storage.EnsureRoot(root);

        string origin = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        string thumbnails = origin + ElFinderRoutes.ThumbnailPath;
        if (root is not null)
        {
            thumbnails += "r/" + FileRoots.ToToken(root) + "/";
        }

        var driver = new FileSystemDriver();
        driver.AddRoot(new RootVolume(
            _storage.ResolvePhysicalPath(root),
            origin + _storage.ResolveRequestPath(root),
            thumbnails)
        {
            IsReadOnly = false,
            IsLocked = false,
            Alias = "Media"
        });

        return new Connector(driver)
        {
            MimeDetect = MimeDetectOption.Internal
        };
    }

    private string? ReadRoot()
    {
        string? value = Request.HasFormContentType ? Request.Form["root"].ToString() : null;
        if (string.IsNullOrWhiteSpace(value))
        {
            value = Request.Query["root"];
        }

        return FileRoots.Normalize(value);
    }

    private static (string? Root, string Hash) SplitThumbnail(string hash)
    {
        const string marker = "r/";
        if (!hash.StartsWith(marker, StringComparison.Ordinal))
        {
            return (null, hash);
        }

        int slash = hash.IndexOf('/', marker.Length);
        if (slash <= marker.Length || slash >= hash.Length - 1)
        {
            return (null, hash);
        }

        return (FileRoots.FromToken(hash[marker.Length..slash]), hash[(slash + 1)..]);
    }
}