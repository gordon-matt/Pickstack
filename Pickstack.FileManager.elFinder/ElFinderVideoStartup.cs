using elFinder.NetCore.Ffmpeg;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pickstack.FileManager.elFinder;

/// <summary>
/// Finds ffmpeg when <see cref="ElFinderOptions.WithVideoThumbnails"/> was used.
/// Failure leaves video thumbnails off.
/// </summary>
internal sealed class ElFinderVideoStartup(
    IOptions<ElFinderOptions> options,
    IHostEnvironment environment,
    ILogger<ElFinderVideoStartup> logger) : IHostedService
{
    /// <summary>
    /// Starts the value.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        ElFinderOptions settings = options.Value;
        if (!settings.VideoThumbnails)
        {
            return Task.CompletedTask;
        }

        var ffmpeg = new FfmpegBootstrapOptions
        {
            PreferLinuxSystemDirectory = true,
            SearchWellKnownLocations = true,
            SearchApplicationDirectories = true,
            DownloadDirectory = Path.Combine(environment.ContentRootPath, "ffmpeg"),
            DownloadIfMissing = true,
            RequireFfprobe = true
        };
        settings.ConfigureVideo?.Invoke(ffmpeg);
        return FfmpegBootstrap.EnsureAsync(ffmpeg, logger, cancellationToken);
    }

    /// <summary>
    /// Stops the value.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
