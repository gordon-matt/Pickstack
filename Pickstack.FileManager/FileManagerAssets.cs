using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Pickstack.FileManager;

public static class FileManagerAssets
{
    public const string RuntimeScriptUrl = "/pickstack/assets/pickstack.js";
    public const string ProviderElementId = "pickstack-file-provider";
    public const string RuntimeItemKey = "Pickstack.RuntimeIncluded";

    private const string StartupItemKey = "Pickstack.FileManager.Startup";

    public static void AppendStartup(StringBuilder builder, HttpContext httpContext, IFileManager fileManager)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(fileManager);

        if (httpContext.Items.ContainsKey(StartupItemKey))
        {
            return;
        }

        httpContext.Items[StartupItemKey] = true;
        AppendRuntime(builder, httpContext);
        AppendAssets(builder, fileManager);
        AppendProvider(builder, fileManager);
    }

    public static string RenderStandaloneStartup(IFileManager fileManager)
    {
        ArgumentNullException.ThrowIfNull(fileManager);

        var builder = new StringBuilder();
        builder.Append("<script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js\"></script>");
        builder.Append("<script src=\"").Append(Encode(RuntimeScriptUrl)).Append("\"></script>");
        AppendAssets(builder, fileManager);
        AppendProvider(builder, fileManager);
        return builder.ToString();
    }

    private static void AppendRuntime(StringBuilder builder, HttpContext httpContext)
    {
        if (httpContext.Items.ContainsKey(RuntimeItemKey))
        {
            return;
        }

        httpContext.Items[RuntimeItemKey] = true;
        builder.Append("<script src=\"").Append(Encode(RuntimeScriptUrl)).Append("\"></script>");
    }

    private static void AppendAssets(StringBuilder builder, IFileManager fileManager)
    {
        foreach (var asset in fileManager.GetClientAssets())
        {
            if (asset.Kind == ClientAssetKind.Style)
            {
                builder.Append("<link rel=\"stylesheet\" href=\"").Append(Encode(asset.Url)).Append("\" />");
            }
            else
            {
                builder.Append("<script src=\"").Append(Encode(asset.Url)).Append("\"></script>");
            }
        }
    }

    private static void AppendProvider(StringBuilder builder, IFileManager fileManager)
    {
        builder.Append("<script type=\"application/json\" id=\"").Append(ProviderElementId).Append("\">");
        builder.Append(fileManager.GetProviderConfiguration());
        builder.Append("</script>");
        builder.Append("<script src=\"").Append(Encode(fileManager.ProviderScriptUrl)).Append("\"></script>");
    }

    private static string Encode(string value) => WebUtility.HtmlEncode(value);
}