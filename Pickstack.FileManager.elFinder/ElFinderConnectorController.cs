using elFinder.NetCore;
using elFinder.NetCore.Drivers.FileSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Pickstack.FileManager.elFinder;

[Route(ElFinderRoutes.Prefix)]
public sealed class ElFinderConnectorController : Controller
{
    private readonly IFileStorage _storage;
    private readonly ILogger<ElFinderConnectorController> _logger;

    public ElFinderConnectorController(IFileStorage storage, ILogger<ElFinderConnectorController> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    [Route("connector")]
    public async Task<IActionResult> Connector()
    {
        try
        {
            return await CreateConnector().ProcessAsync(Request);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "elFinder could not process a connector request.");
            return Json(new { error = "Unable to process the file manager request." });
        }
    }

    [Route("thumb/{*hash}")]
    public async Task<IActionResult> Thumbnail(string hash)
    {
        try
        {
            return await CreateConnector().GetThumbnailAsync(Request, Response, hash);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "elFinder could not create a thumbnail.");
            return Json(new { error = "Unable to process the file manager request." });
        }
    }

    private Connector CreateConnector()
    {
        _storage.EnsureCreated();

        string origin = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        var driver = new FileSystemDriver();
        driver.AddRoot(new RootVolume(
            _storage.PhysicalPath,
            origin + _storage.RequestPath,
            origin + ElFinderRoutes.ThumbnailPath)
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
}