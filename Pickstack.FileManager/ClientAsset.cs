namespace Pickstack.FileManager;

public enum ClientAssetKind
{
    Style,
    Script
}

/// <summary>
/// A stylesheet or script the host page must load for a component.
/// </summary>
public sealed class ClientAsset
{
    public required string Url { get; init; }

    public required ClientAssetKind Kind { get; init; }
}