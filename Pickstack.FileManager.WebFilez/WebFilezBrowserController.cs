using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pickstack.FileManager;

namespace Pickstack.FileManager.WebFilez;

public sealed class WebFilezBrowserModel
{
    public required string RootId { get; init; }

    public string? Path { get; init; }

    public required string Theme { get; init; }

    public string? Language { get; init; }

    public required string SelectionMode { get; init; }

    public string? Kind { get; init; }

    public string? Accept { get; init; }

    public required string SelectLabel { get; init; }

    public bool ShowTree { get; init; }

    /// <summary>
    /// Editor map for this instance. Empty leaves the rich text picker without FileRobot, SuperDoc, or Fortune Sheet.
    /// </summary>
    public string? Editors { get; init; }
}

[Route(WebFilezRoutes.Prefix)]
public sealed class WebFilezBrowserController : Controller
{
    public const string StandaloneEditors =
        "filerobot:.jpg,.jpeg,.png,.webp,.gif,.bmp,.avif|superdoc:.docx|fortunesheet:.xlsx,.csv";

    private readonly WebFilezFileManagerOptions _options;
    private readonly IFileStorage _storage;

    public WebFilezBrowserController(IOptions<WebFilezFileManagerOptions> options, IFileStorage storage)
    {
        _options = options.Value;
        _storage = storage;
    }

    [HttpGet]
    public IActionResult Index(string? type, bool select, string? root, bool editors)
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
            Theme = _options.Theme,
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

    private static string? KindFor(string type) => type switch
    {
        "image" => "images",
        "media" => "media",
        "folder" => "folders",
        _ => null
    };
}
