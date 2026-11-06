namespace Pickstack.Abstractions;

/// <summary>
/// Specifies the client asset kind.
/// </summary>
public enum ClientAssetKind
{
    /// <summary>
    /// The style value.
    /// </summary>
    Style,
    /// <summary>
    /// The script value.
    /// </summary>
    Script
}

/// <summary>
/// A stylesheet or script the host page must load for a component.
/// </summary>
public sealed class ClientAsset
{
    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// Gets or sets the kind.
    /// </summary>
    public required ClientAssetKind Kind { get; init; }
}