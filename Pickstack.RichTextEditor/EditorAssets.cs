using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Pickstack.RichTextEditor;

public static class EditorAssets
{
    public const string RuntimeScriptUrl = "/pickstack/assets/pickstack.js";
    public const string RuntimeItemKey = "Pickstack.RuntimeIncluded";

    private const string StartupItemKey = "Pickstack.Editor.Startup";

    public static void AppendStartup(StringBuilder builder, HttpContext httpContext, IRichTextEditor editor)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(editor);

        if (httpContext.Items.ContainsKey(StartupItemKey))
        {
            return;
        }

        httpContext.Items[StartupItemKey] = true;

        if (!httpContext.Items.ContainsKey(RuntimeItemKey))
        {
            httpContext.Items[RuntimeItemKey] = true;
            builder.Append("<script src=\"").Append(WebUtility.HtmlEncode(RuntimeScriptUrl)).Append("\"></script>");
        }

        foreach (var asset in editor.GetClientAssets())
        {
            if (asset.Kind == ClientAssetKind.Style)
            {
                builder.Append("<link rel=\"stylesheet\" href=\"").Append(WebUtility.HtmlEncode(asset.Url)).Append("\" />");
            }
            else
            {
                builder.Append("<script src=\"").Append(WebUtility.HtmlEncode(asset.Url)).Append("\"></script>");
            }
        }

        builder.Append("<script src=\"").Append(WebUtility.HtmlEncode(editor.ProviderScriptUrl)).Append("\"></script>");
    }
}