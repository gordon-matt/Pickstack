using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pickstack.FileManager;

namespace Pickstack.FileManager.WebFilez;

/// <summary>
/// Represents the web filez browser model.
/// </summary>
public sealed class WebFilezBrowserModel
{
    /// <summary>
    /// Gets or sets the root identifier.
    /// </summary>
    public required string RootId { get; init; }

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    public string? Path { get; init; }

    /// <summary>
    /// Gets or sets the theme.
    /// </summary>
    public required string Theme { get; init; }

    /// <summary>
    /// Gets or sets the language.
    /// </summary>
    public string? Language { get; init; }

    /// <summary>
    /// Gets or sets the selection mode.
    /// </summary>
    public required string SelectionMode { get; init; }

    /// <summary>
    /// Gets or sets the kind.
    /// </summary>
    public string? Kind { get; init; }

    /// <summary>
    /// Gets or sets the accept.
    /// </summary>
    public string? Accept { get; init; }

    /// <summary>
    /// Gets or sets the select label.
    /// </summary>
    public required string SelectLabel { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is show tree.
    /// </summary>
    public bool ShowTree { get; init; }

    /// <summary>
    /// Editor map for this instance. Empty leaves the rich text picker without FileRobot, SuperDoc, or Fortune Sheet.
    /// </summary>
    public string? Editors { get; init; }
}

/// <summary>
/// Represents the web filez browser controller.
/// </summary>
[Route(WebFilezRoutes.Prefix)]
public sealed class WebFilezBrowserController : Controller
{
    /// <summary>
    /// The standalone editors constant.
    /// </summary>
    public const string StandaloneEditors =
        "filerobot:.jpg,.jpeg,.png,.webp,.gif,.bmp,.avif|superdoc:.docx|fortunesheet:.xlsx,.csv";

    private readonly WebFilezFileManagerOptions _options;
    private readonly IFileStorage _storage;

    /// <summary>
    /// Initializes a new instance of the WebFilezBrowserController class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="storage">The storage.</param>
    public WebFilezBrowserController(IOptions<WebFilezFileManagerOptions> options, IFileStorage storage)
    {
        _options = options.Value;
        _storage = storage;
    }

    /// <summary>
    /// Gets the index.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="select">The select.</param>
    /// <param name="root">The root.</param>
    /// <param name="editors">The editors.</param>
    /// <param name="theme">The theme.</param>
    /// <returns>The index.</returns>
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
