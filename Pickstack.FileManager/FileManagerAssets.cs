using System.Text;
using Microsoft.AspNetCore.Http;
using Pickstack.Abstractions;

namespace Pickstack.FileManager;

/// <summary>Markup for the registered file manager and the shared runtime.</summary>
public static class FileManagerAssets
{
    /// <summary>Address of the shared browser script. The same value as <see cref="ClientAssets.RuntimeScriptUrl"/>.</summary>
    public const string RuntimeScriptUrl = ClientAssets.RuntimeScriptUrl;

    /// <summary>The <c>id</c> of the JSON element the file-manager provider script reads. The value is <c>pickstack-file-provider</c>.</summary>
    public const string ProviderElementId = "pickstack-file-provider";

    /// <summary>Item key that records the runtime script as already written. The same value as <see cref="ClientAssets.RuntimeItemKey"/>.</summary>
    public const string RuntimeItemKey = ClientAssets.RuntimeItemKey;

    private const string StartupItemKey = "Pickstack.FileManager.Startup";

    /// <summary>
    /// Appends the runtime and this file manager's includes once per request.
    /// A second call for the same request writes nothing.
    /// </summary>
    /// <param name="builder">The markup being built.</param>
    /// <param name="httpContext">The request. Its items remember that the startup was written.</param>
    /// <param name="fileManager">The registered file manager.</param>
    /// <exception cref="ArgumentNullException">Any argument is <see langword="null"/>.</exception>
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

    /// <summary>
    /// Markup for a page that has no host layout: jQuery from the CDN, the shared runtime, and the file manager.
    /// The CKEditor browse window uses this.
    /// </summary>
    /// <param name="fileManager">The registered file manager.</param>
    /// <returns>The elements.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileManager"/> is <see langword="null"/>.</exception>
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