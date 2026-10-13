namespace Pickstack;

public sealed class PickstackAsset
{
    public required string Name { get; init; }

    public required string ContentType { get; init; }

    public required Func<Stream> OpenRead { get; init; }
}

public interface IPickstackAssetSource
{
    IEnumerable<PickstackAsset> GetAssets();
}
