using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pickstack.FileManager;

namespace Pickstack.FileManager.WebFilez;

/// <summary>Values the WebFilez browser view reads for one request.</summary>
public sealed class WebFilezBrowserModel
{
    /// <summary>The WebFilez root id of the shared upload folder.</summary>
    public required string RootId { get; init; }

    /// <summary>The relative folder to open, or <see langword="null"/> for the root itself. The user cannot navigate above it.</summary>
    public string? Path { get; init; }

    /// <summary><c>light</c>, <c>dark</c>, <c>auto</c>, or <c>bootstrap</c>.</summary>
    public required string Theme { get; init; }

    /// <summary>The IETF language tag, or <see langword="null"/> to follow the UI culture.</summary>
    public string? Language { get; init; }

    /// <summary>Single selection when a file is being chosen. None for a folder or a browser that is not choosing.</summary>
    public required string SelectionMode { get; init; }

    /// <summary><c>images</c>, <c>media</c>, or <c>folders</c>. <see langword="null"/> leaves every kind visible.</summary>
    public string? Kind { get; init; }

    /// <summary><c>image/*</c> for an image field. <see langword="null"/> for every other type.</summary>
    public string? Accept { get; init; }

    /// <summary>Text of the button that chooses the open file.</summary>
    public required string SelectLabel { get; init; }

    /// <summary><see langword="true"/> shows the folder tree.</summary>
    public bool ShowTree { get; init; }

    /// <summary>
    /// Editor map for this instance. Empty leaves the rich text picker without FileRobot, SuperDoc, or Fortune Sheet.
    /// </summary>
    public string? Editors { get; init; }
}

/// <summary>The frame page that hosts the WebFilez browser.</summary>
[Route(WebFilezRoutes.Prefix)]
public sealed class WebFilezBrowserController : Controller
{
    /// <summary>
    /// Editor map for a standalone browser: FileRobot for images, SuperDoc for <c>.docx</c>, and Fortune Sheet for <c>.xlsx</c> and <c>.csv</c>.
    /// </summary>
    public const string StandaloneEditors =
        "filerobot:.jpg,.jpeg,.png,.webp,.gif,.bmp,.avif|superdoc:.docx|fortunesheet:.xlsx,.csv";

    private readonly WebFilezFileManagerOptions _options;
    private readonly IFileStorage _storage;

    /// <summary>Reads the browser options and the shared upload folder.</summary>
    /// <param name="options">The WebFilez options.</param>
    /// <param name="storage">The shared upload folder. A requested root is created here.</param>
    public WebFilezBrowserController(IOptions<WebFilezFileManagerOptions> options, IFileStorage storage)
    {
        _options = options.Value;
        _storage = storage;
    }

    /// <summary>
    /// Renders the browser. A blank <paramref name="type"/> is <c>any</c>.
    /// <paramref name="select"/> chooses a single file, except for a folder, which writes the open folder instead.
    /// An unknown <paramref name="theme"/> keeps the configured theme. <c>light</c>, <c>dark</c>, <c>auto</c>, and <c>bootstrap</c> are accepted.
    /// </summary>
    /// <param name="type"><c>any</c>, <c>file</c>, <c>image</c>, <c>media</c>, or <c>folder</c>.</param>
    /// <param name="select"><see langword="true"/> when a file is being chosen.</param>
    /// <param name="root">The relative folder to open. It is created when it is valid.</param>
    /// <param name="editors"><see langword="true"/> enables the standalone image, document, and spreadsheet editors.</param>
    /// <param name="theme">A theme for this request, or <see langword="null"/> for the configured theme.</param>
    /// <returns>The browser view, or 400 when <paramref name="root"/> is not a valid file root.</returns>
    [HttpGet]
    public IActionResult Index(string? type, bool select, string? root, bool editors, string? theme)
    {
        string? path = null;
        if (!string.IsNullOrWhiteSpace(root))
        {
            try
            {
                path = FileRoots.Normalize(root);
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }

            _storage.EnsureRoot(path);
        }

        string normalized = string.IsNullOrWhiteSpace(type) ? "any" : type.ToLowerInvariant();
        return View(new WebFilezBrowserModel
        {
            RootId = _options.RootId,
            Path = path,
            Theme = ThemeOrDefault(theme),
            Language = _options.Language,
            SelectionMode = select && normalized != "folder"
                ? global::WebFilez.Constants.SelectionModes.Single
                : global::WebFilez.Constants.SelectionModes.None,
            Kind = KindFor(normalized),
            Accept = normalized == "image" ? "image/*" : null,
            SelectLabel = _options.SelectLabel,
            ShowTree = _options.ShowTree,
            Editors = editors ? StandaloneEditors : null
        });
    }

    private string ThemeOrDefault(string? theme) =>
        theme is "light" or "dark" or "auto" or "bootstrap" ? theme : _options.Theme;

    private static string? KindFor(string type) => type switch
    {
        "image" => "images",
        "media" => "media",
        "folder" => "folders",
        _ => null
    };
}
