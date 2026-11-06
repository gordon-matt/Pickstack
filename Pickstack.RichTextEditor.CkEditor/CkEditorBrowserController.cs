using Microsoft.AspNetCore.Mvc;
using Pickstack.FileManager;

namespace Pickstack.RichTextEditor.CkEditor;

/// <summary>
/// Provides ck editor routes.
/// </summary>
public static class CkEditorRoutes
{
    /// <summary>
    /// The prefix constant.
    /// </summary>
    public const string Prefix = "pickstack/ckeditor";

    /// <summary>
    /// The browse path constant.
    /// </summary>
    public const string BrowsePath = "/pickstack/ckeditor/browse";
}

/// <summary>
/// Represents the ck editor browser controller.
/// </summary>
[Route(CkEditorRoutes.Prefix)]
public sealed class CkEditorBrowserController : Controller
{
    private readonly IFileManager _fileManager;

    /// <summary>
    /// Initializes a new instance of the CkEditorBrowserController class.
    /// </summary>
    /// <param name="fileManager">The file manager.</param>
    public CkEditorBrowserController(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    /// <summary>
    /// Gets the browse.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="root">The root.</param>
    /// <returns>The browse.</returns>
    [HttpGet("browse")]
    public ContentResult Browse(string? type, string? root)
    {
        string fileType = NormalizeType(type);
        string rootAttribute = string.IsNullOrWhiteSpace(root)
            ? ""
            : $" data-file-root=\"{System.Net.WebUtility.HtmlEncode(root)}\"";
        string html = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1" />
                <title>Select a file</title>
                <style>
                    html, body { height: 100%; margin: 0; }
                    #pickstack-browser { height: 100%; }
                </style>
                {{FileManagerAssets.RenderStandaloneStartup(_fileManager)}}
            </head>
            <body>
                <div id="pickstack-browser" data-pickstack-browser="true" data-select="true" data-file-type="{{fileType}}"{{rootAttribute}}></div>
                <script>
                    (function () {
                        var funcNum = new URLSearchParams(window.location.search).get("CKEditorFuncNum");
                        Pickstack.files.onSelect(function (file) {
                            if (window.opener && window.opener.CKEDITOR && funcNum) {
                                window.opener.CKEDITOR.tools.callFunction(parseInt(funcNum, 10), file.url);
                                window.close();
                            }
                        });
                    })();
                </script>
            </body>
            </html>
            """;

        return Content(html, "text/html");
    }

    private static string NormalizeType(string? type) => type?.ToLowerInvariant() switch
    {
        "image" => "image",
        "media" => "media",
        "file" => "file",
        "folder" => "folder",
        _ => "any"
    };
}