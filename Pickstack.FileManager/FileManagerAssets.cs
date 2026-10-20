using System.Text;
using Microsoft.AspNetCore.Http;
using Pickstack.Abstractions;

namespace Pickstack.FileManager;

public static class FileManagerAssets
{
    public const string RuntimeScriptUrl = ClientAssets.RuntimeScriptUrl;
    public const string ProviderElementId = "pickstack-file-provider";
    public const string RuntimeItemKey = ClientAssets.RuntimeItemKey;

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
        Append(builder, httpContext, fileManager);
    }

    public static string RenderStandaloneStartup(IFileManager fileManager)
    {
        ArgumentNullException.ThrowIfNull(fileManager);

        var builder = new StringBuilder();
        builder.Append("<script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js\"></script>");
        ClientAssets.AppendRuntime(builder);
        ClientAssets.Append(builder, Includes(fileManager));
        return builder.ToString();
    }

    /// <summary>Styles, scripts, and provider configuration for the registered file manager. Does not include the shared runtime.</summary>
    public static IEnumerable<ClientInclude> Includes(IFileManager fileManager)
    {
        ArgumentNullException.ThrowIfNull(fileManager);

        foreach (var asset in fileManager.GetClientAssets())
        {
            yield return asset.Kind == ClientAssetKind.Style
                ? ClientInclude.Style(asset.Url)
                : ClientInclude.Script(asset.Url);
        }

        yield return ClientInclude.Json(ProviderElementId, fileManager.GetProviderConfiguration());
        yield return ClientInclude.Script(fileManager.ProviderScriptUrl);
    }

    private static void Append(StringBuilder builder, HttpContext httpContext, IFileManager fileManager)
    {
        ClientAssets.AppendRuntimeOnce(builder, httpContext.Items);
        ClientAssets.Append(builder, Includes(fileManager));
    }
}