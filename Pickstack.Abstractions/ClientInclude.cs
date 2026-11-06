namespace Pickstack.Abstractions;

/// <summary>
/// One stylesheet, script, or JSON configuration block a host page must emit.
/// </summary>
public sealed class ClientInclude
{
    /// <summary>
    /// Gets or sets the kind.
    /// </summary>
    public string Kind { get; init; } = "";

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Gets the style.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The style.</returns>
    public static ClientInclude Style(string url) => new() { Kind = "style", Url = url };

    /// <summary>
    /// Gets the script.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The script.</returns>
    public static ClientInclude Script(string url) => new() { Kind = "script", Url = url };

    /// <summary>
    /// Gets the JSON.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="content">The content.</param>
    /// <returns>The JSON.</returns>
    public static ClientInclude Json(string id, string content) => new() { Kind = "json", Id = id, Content = content };
}
