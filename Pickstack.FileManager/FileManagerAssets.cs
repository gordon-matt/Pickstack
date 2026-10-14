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
        ClientAssets.Append(builder, fileManager.GetClientAssets());
        ClientAssets.AppendProvider(builder, ProviderElementId, fileManager.GetProviderConfiguration(), fileManager.ProviderScriptUrl);
        return builder.ToString();
    }

    private static void Append(StringBuilder builder, HttpContext httpContext, IFileManager fileManager)
    {
        ClientAssets.AppendRuntimeOnce(builder, httpContext.Items);
        ClientAssets.Append(builder, fileManager.GetClientAssets());
        ClientAssets.AppendProvider(builder, ProviderElementId, fileManager.GetProviderConfiguration(), fileManager.ProviderScriptUrl);
    }
}