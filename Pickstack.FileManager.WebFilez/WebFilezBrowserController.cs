using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
}

[Route(WebFilezRoutes.Prefix)]
public sealed class WebFilezBrowserController : Controller
{
    private readonly WebFilezFileManagerOptions _options;

    public WebFilezBrowserController(IOptions<WebFilezFileManagerOptions> options)
    {
        _options = options.Value;
    }

    [HttpGet]
    public IActionResult Index(string? type, bool select, string? root)
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
            ShowTree = _options.ShowTree
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
