using System.Text;
using Microsoft.AspNetCore.Http;
using Pickstack.Abstractions;

namespace Pickstack.RichTextEditor;

/// <summary>
/// Provides editor assets.
/// </summary>
public static class EditorAssets
{
    /// <summary>
    /// The runtime script URL constant.
    /// </summary>
    public const string RuntimeScriptUrl = ClientAssets.RuntimeScriptUrl;
    /// <summary>
    /// The runtime item key constant.
    /// </summary>
    public const string RuntimeItemKey = ClientAssets.RuntimeItemKey;
    /// <summary>
    /// The provider element identifier constant.
    /// </summary>
    public const string ProviderElementId = "pickstack-editor-provider";

    private const string StartupItemKey = "Pickstack.Editor.Startup";

    /// <summary>
    /// Appends the startup.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="editor">The editor.</param>
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
        ClientAssets.AppendRuntimeOnce(builder, httpContext.Items);
        ClientAssets.Append(builder, Includes(editor));
    }

    /// <summary>Styles, scripts, and provider configuration for the registered editor. Does not include the shared runtime.</summary>
    public static IEnumerable<ClientInclude> Includes(IRichTextEditor editor)
    {
        ArgumentNullException.ThrowIfNull(editor);

        foreach (var asset in editor.GetClientAssets())
        {
            yield return asset.Kind == ClientAssetKind.Style
                ? ClientInclude.Style(asset.Url)
                : ClientInclude.Script(asset.Url);
        }

        yield return ClientInclude.Json(ProviderElementId, editor.GetProviderConfiguration());
        yield return ClientInclude.Script(editor.ProviderScriptUrl);
    }
}