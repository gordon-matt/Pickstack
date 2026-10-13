namespace Pickstack.RichTextEditor;

public enum ClientAssetKind
{
    Style,
    Script
}

public sealed class ClientAsset
{
    public required string Url { get; init; }

    public required ClientAssetKind Kind { get; init; }
}