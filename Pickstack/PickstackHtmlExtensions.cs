using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Pickstack.FileManager;
using Pickstack.RichTextEditor;

namespace Pickstack;

/// <summary>Writes the registered file manager and editor into a Razor view.</summary>
public static class PickstackHtmlExtensions
{
    /// <summary>
    /// Writes the shared runtime and the scripts for the registered file manager and rich text editor.
    /// A component that is not registered is omitted. When neither is registered, the result is empty and the runtime is omitted too.
    /// The runtime is written once when both are registered.
    /// </summary>
    /// <param name="html">The view helper.</param>
    /// <returns>The markup.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="html"/> is <see langword="null"/>.</exception>
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
