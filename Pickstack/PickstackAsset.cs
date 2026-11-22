namespace Pickstack;

/// <summary>One file served from <c>/pickstack/assets/{name}</c>.</summary>
public sealed class PickstackAsset
{
    /// <summary>The name matched against the request. The match ignores case.</summary>
    public required string Name { get; init; }

    /// <summary>The response content type.</summary>
    public required string ContentType { get; init; }

    /// <summary>Opens the body. The caller disposes the stream.</summary>
    public required Func<Stream> OpenRead { get; init; }
}

/// <summary>Embedded files an adapter adds to <c>/pickstack/assets</c>.</summary>
public interface IPickstackAssetSource
{
    /// <summary>The files this source contributes. The first source that matches a name is served.</summary>
    /// <returns>The assets. The sequence can be empty.</returns>
    IEnumerable<PickstackAsset> GetAssets();
}