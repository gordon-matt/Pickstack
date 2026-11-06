namespace Pickstack;

/// <summary>
/// Represents the pickstack asset.
/// </summary>
public sealed class PickstackAsset
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the content type.
    /// </summary>
    public required string ContentType { get; init; }

    /// <summary>
    /// Gets or sets the open read.
    /// </summary>
    public required Func<Stream> OpenRead { get; init; }
}

/// <summary>
/// Defines the contract for the pickstack asset source.
/// </summary>
public interface IPickstackAssetSource
{
    /// <summary>
    /// Gets the assets.
    /// </summary>
    /// <returns>The assets.</returns>
    IEnumerable<PickstackAsset> GetAssets();
}