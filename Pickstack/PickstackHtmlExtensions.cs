using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Pickstack.FileManager;
using Pickstack.RichTextEditor;

namespace Pickstack;

public static class PickstackHtmlExtensions
{
    /// <summary>
    /// Writes the shared Pickstack runtime and the scripts for the registered file manager and rich text editor.
    /// </summary>
    public static IHtmlContent RenderPickstackScripts(this IHtmlHelper html)
    {
        ArgumentNullException.ThrowIfNull(html);

        var httpContext = html.ViewContext.HttpContext;
        var builder = new StringBuilder();
        var fileManager = httpContext.RequestServices.GetService<IFileManager>();
        var editor = httpContext.RequestServices.GetService<IRichTextEditor>();
        if (fileManager is not null)
        {
            FileManagerAssets.AppendStartup(builder, httpContext, fileManager);
        }

        if (editor is not null)
        {
            EditorAssets.AppendStartup(builder, httpContext, editor);
        }

        return new HtmlString(builder.ToString());
    }
}
