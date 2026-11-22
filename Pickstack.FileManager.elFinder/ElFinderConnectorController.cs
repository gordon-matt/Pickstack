using elFinder.NetCore;
using elFinder.NetCore.Drivers.FileSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Pickstack.FileManager.elFinder;

/// <summary>
/// The elFinder connector. A request can name a <c>root</c> folder in the form or the query.
/// A failure is logged and returned as JSON with the message "Unable to process the file manager request."
/// </summary>
[Route(ElFinderRoutes.Prefix)]
public sealed class ElFinderConnectorController : Controller
{
    private readonly IFileStorage _storage;
    private readonly ILogger<ElFinderConnectorController> _logger;

    /// <summary>Uses the shared upload folder as the elFinder volume named Media.</summary>
    /// <param name="storage">The shared upload folder.</param>
    /// <param name="logger">Receives connector and thumbnail failures.</param>
    public ElFinderConnectorController(IFileStorage storage, ILogger<ElFinderConnectorController> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    /// <summary>
    /// Handles one elFinder command against the requested root. The root is created when it is missing.
    /// The volume is writable, its alias is Media, and its URL is the shared request path.
    /// </summary>
    /// <returns>The connector response, or the error JSON when the command fails.</returns>
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
    /// Returns one thumbnail. A hash that starts with <c>r/{token}/</c> is read from that file root. Any other hash uses the upload folder.
    /// </summary>
    /// <param name="hash">The elFinder file hash, optionally prefixed with a file-root token.</param>
    /// <returns>The thumbnail, or the error JSON when it cannot be created.</returns>
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