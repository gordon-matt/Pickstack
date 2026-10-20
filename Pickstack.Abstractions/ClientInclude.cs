namespace Pickstack.Abstractions;

/// <summary>
/// One stylesheet, script, or JSON configuration block a host page must emit.
/// </summary>
public sealed class ClientInclude
{
    public string Kind { get; init; } = "";

    public string? Url { get; init; }

    public string? Id { get; init; }

    public string? Content { get; init; }

    public static ClientInclude Style(string url) => new() { Kind = "style", Url = url };

    public static ClientInclude Script(string url) => new() { Kind = "script", Url = url };

    public static ClientInclude Json(string id, string content) => new() { Kind = "json", Id = id, Content = content };
}
