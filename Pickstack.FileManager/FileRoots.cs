using System.Text;

namespace Pickstack.FileManager;

/// <summary>
/// A relative folder under the shared upload directory, such as <c>42</c> or <c>42/7</c>.
/// Segments cannot contain <c>.</c> because Responsive File Manager ignores a root that does.
/// </summary>
public static class FileRoots
{
    /// <summary>
    /// Normalizes the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The string.</returns>
    public static string? Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        string[] segments = value.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (segments.Length == 0 || segments.Any(IsUnsafe))
        {
            throw new InvalidOperationException($"'{value}' is not a valid file root.");
        }

        return string.Join('/', segments);
    }

    /// <summary>
    /// Converts the value to the token.
    /// </summary>
    /// <param name="root">The root.</param>
    /// <returns>The token.</returns>
    public static string ToToken(string root)
    {
        string normalized = Normalize(root)
            ?? throw new InvalidOperationException("A file root is required.");

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(normalized))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    /// <summary>
    /// Creates a value from the token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>The token.</returns>
    public static string FromToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        string padded = token.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + (4 - padded.Length % 4) % 4, '=');
        return Normalize(Encoding.UTF8.GetString(Convert.FromBase64String(padded)))
            ?? throw new InvalidOperationException("The file root token is empty.");
    }

    /// <summary>
    /// Determines whether this instance is unsafe.
    /// </summary>
    /// <param name="segment">The segment.</param>
    /// <returns>true when the condition is met; otherwise, false.</returns>
    public static bool IsUnsafe(string segment) =>
        segment is "." or ".."
        || segment.Contains(':', StringComparison.Ordinal)
        || segment.Contains('.', StringComparison.Ordinal);
}