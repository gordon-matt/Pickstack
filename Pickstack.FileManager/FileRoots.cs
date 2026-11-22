using System.Text;

namespace Pickstack.FileManager;

/// <summary>
/// A relative folder under the shared upload directory, such as <c>42</c> or <c>42/7</c>.
/// Segments cannot contain <c>.</c> because Responsive File Manager ignores a root that does.
/// </summary>
public static class FileRoots
{
    /// <summary>
    /// Trims the folder, accepts <c>/</c> or <c>\</c>, and joins the segments with <c>/</c>.
    /// A blank value is the upload folder itself and returns <see langword="null"/>.
    /// </summary>
    /// <param name="value">The relative folder.</param>
    /// <returns>The normalized folder, or <see langword="null"/> when <paramref name="value"/> is blank.</returns>
    /// <exception cref="InvalidOperationException">A segment is <c>.</c>, <c>..</c>, or contains <c>.</c> or <c>:</c>. The message is "'{value}' is not a valid file root."</exception>
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

    /// <summary>Encodes a normalized root as unpadded base64, with <c>+</c> and <c>/</c> replaced so the token can sit in a URL path.</summary>
    /// <param name="root">The relative folder.</param>
    /// <returns>The token.</returns>
    /// <exception cref="InvalidOperationException"><paramref name="root"/> is blank or not a valid file root. A blank root says "A file root is required."</exception>
    public static string ToToken(string root)
    {
        string normalized = Normalize(root)
            ?? throw new InvalidOperationException("A file root is required.");

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(normalized))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    /// <summary>Decodes a token from <see cref="ToToken"/> and normalizes the folder it contains.</summary>
    /// <param name="token">The token.</param>
    /// <returns>The relative folder.</returns>
    /// <exception cref="ArgumentException"><paramref name="token"/> is <see langword="null"/> or white space.</exception>
    /// <exception cref="FormatException"><paramref name="token"/> is not base64.</exception>
    /// <exception cref="InvalidOperationException">The decoded folder is empty or not a valid file root.</exception>
    public static string FromToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        string padded = token.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + (4 - padded.Length % 4) % 4, '=');
        return Normalize(Encoding.UTF8.GetString(Convert.FromBase64String(padded)))
            ?? throw new InvalidOperationException("The file root token is empty.");
    }

    /// <summary>Whether one path segment would leave the upload folder or contains a dot. Responsive File Manager ignores a root that contains a dot.</summary>
    /// <param name="segment">One folder name, without separators.</param>
    /// <returns><see langword="true"/> for <c>.</c>, <c>..</c>, or a segment that contains <c>.</c> or <c>:</c>.</returns>
    public static bool IsUnsafe(string segment) =>
        segment is "." or ".."
        || segment.Contains(':', StringComparison.Ordinal)
        || segment.Contains('.', StringComparison.Ordinal);
}