namespace Pickstack.Abstractions;

/// <summary>
/// One stylesheet, script, or JSON configuration block a host page must emit.
/// </summary>
public sealed class ClientInclude
{
    /// <summary><c>style</c>, <c>script</c>, or <c>json</c>. Any other value is ignored when the include is rendered.</summary>
    public string Kind { get; init; } = "";

    /// <summary>Address of a stylesheet or script. Unused for JSON.</summary>
    public string? Url { get; init; }

    /// <summary>The <c>id</c> of a JSON script element. Unused for a stylesheet or script.</summary>
    public string? Id { get; init; }

    /// <summary>JSON written inside the script element. It is not HTML-encoded, so it must already be HTML-safe.</summary>
    public string? Content { get; init; }

    /// <summary>A stylesheet include.</summary>
    /// <param name="url">Address of the stylesheet.</param>
    /// <returns>An include whose kind is <c>style</c>.</returns>
    public static ClientInclude Style(string url) => new() { Kind = "style", Url = url };

    /// <summary>A script include.</summary>
    /// <param name="url">Address of the script.</param>
    /// <returns>An include whose kind is <c>script</c>.</returns>
    public static ClientInclude Script(string url) => new() { Kind = "script", Url = url };

    /// <summary>A JSON configuration block.</summary>
    /// <param name="id">The <c>id</c> the provider script looks up.</param>
    /// <param name="content">The JSON. It is written as supplied.</param>
    /// <returns>An include whose kind is <c>json</c>.</returns>
    public static ClientInclude Json(string id, string content) => new() { Kind = "json", Id = id, Content = content };
}
