using System.Text;
using Microsoft.AspNetCore.Http;
using Pickstack.Abstractions;

namespace Pickstack.RichTextEditor;

public static class EditorAssets
{
    public const string RuntimeScriptUrl = ClientAssets.RuntimeScriptUrl;
    public const string RuntimeItemKey = ClientAssets.RuntimeItemKey;
    public const string ProviderElementId = "pickstack-editor-provider";

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
        ClientAssets.AppendRuntimeOnce(builder, httpContext.Items);
        ClientAssets.Append(builder, editor.GetClientAssets());
        ClientAssets.AppendProvider(builder, ProviderElementId, editor.GetProviderConfiguration(), editor.ProviderScriptUrl);
    }
}