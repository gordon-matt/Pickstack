using System.Text;
using Microsoft.AspNetCore.Http;
using Pickstack.Abstractions;

namespace Pickstack.RichTextEditor;

/// <summary>Markup for the registered editor and the shared runtime.</summary>
public static class EditorAssets
{
    /// <summary>Address of the shared browser script. The same value as <see cref="ClientAssets.RuntimeScriptUrl"/>.</summary>
    public const string RuntimeScriptUrl = ClientAssets.RuntimeScriptUrl;

    /// <summary>Item key that records the runtime script as already written. The same value as <see cref="ClientAssets.RuntimeItemKey"/>.</summary>
    public const string RuntimeItemKey = ClientAssets.RuntimeItemKey;

    /// <summary>The <c>id</c> of the JSON element the editor provider script reads. The value is <c>pickstack-editor-provider</c>.</summary>
    public const string ProviderElementId = "pickstack-editor-provider";

    private const string StartupItemKey = "Pickstack.Editor.Startup";

    /// <summary>
    /// Appends the runtime and this editor's includes once per request.
    /// A second editor on the same request writes nothing. The runtime is also skipped when a file manager already wrote it.
    /// </summary>
    /// <param name="builder">The markup being built.</param>
    /// <param name="httpContext">The request. Its items remember that the startup was written.</param>
    /// <param name="editor">The registered editor.</param>
    /// <exception cref="ArgumentNullException">Any argument is <see langword="null"/>.</exception>
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