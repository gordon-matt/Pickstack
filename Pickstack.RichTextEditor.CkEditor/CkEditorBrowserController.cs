using Microsoft.AspNetCore.Mvc;
using Pickstack.FileManager;

namespace Pickstack.RichTextEditor.CkEditor;

public static class CkEditorRoutes
{
    public const string Prefix = "pickstack/ckeditor";

    public const string BrowsePath = "/pickstack/ckeditor/browse";
}

[Route(CkEditorRoutes.Prefix)]
public sealed class CkEditorBrowserController : Controller
{
    private readonly IFileManager _fileManager;

    public CkEditorBrowserController(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    [HttpGet("browse")]
    public ContentResult Browse(string? type)
    {
        string fileType = NormalizeType(type);
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
                <div id="pickstack-browser" data-pickstack-browser="true" data-select="true" data-file-type="{{fileType}}"></div>
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
        _ => "any"
    };
}