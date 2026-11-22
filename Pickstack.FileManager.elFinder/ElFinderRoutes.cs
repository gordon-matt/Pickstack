namespace Pickstack.FileManager.elFinder;

/// <summary>Addresses of the elFinder connector and its thumbnails.</summary>
public static class ElFinderRoutes
{
    /// <summary>Route prefix of the connector controller. The value is <c>pickstack/elfinder</c>.</summary>
    public const string Prefix = "pickstack/elfinder";

    /// <summary>The connector the browser calls. The value is <c>/pickstack/elfinder/connector</c>.</summary>
    public const string ConnectorPath = "/pickstack/elfinder/connector";

    /// <summary>Thumbnail prefix. A file-root token is inserted after <c>thumb/r/</c>. The value is <c>/pickstack/elfinder/thumb/</c>.</summary>
    public const string ThumbnailPath = "/pickstack/elfinder/thumb/";
}