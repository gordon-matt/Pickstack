namespace Pickstack.Abstractions;

/// <summary>
/// Specifies the client asset kind.
/// </summary>
public enum ClientAssetKind
{
    /// <summary>A stylesheet. It is emitted as a <c>link</c> element.</summary>
    Style,

    /// <summary>A script. It is emitted as a <c>script</c> element with a <c>src</c>.</summary>
    Script
}

/// <summary>
/// A stylesheet or script the host page must load for a component.
/// </summary>
public sealed class ClientAsset
{
    /// <summary>Address of the stylesheet or script.</summary>
    public required string Url { get; init; }

    /// <summary>Whether <see cref="Url"/> is a stylesheet or a script.</summary>
    public required ClientAssetKind Kind { get; init; }
}