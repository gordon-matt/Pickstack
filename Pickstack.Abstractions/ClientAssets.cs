using System.Net;
using System.Text;

namespace Pickstack.Abstractions;

/// <summary>
/// Shared markup for the Pickstack browser runtime and a component's client assets.
/// </summary>
public static class ClientAssets
{
    public const string RuntimeScriptUrl = "/pickstack/assets/pickstack.js";

    public const string RuntimeItemKey = "Pickstack.RuntimeIncluded";

    public static void AppendRuntime(StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        AppendScript(builder, RuntimeScriptUrl);
    }

    public static void AppendRuntimeOnce(StringBuilder builder, IDictionary<object, object?> items)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(items);

        if (items.ContainsKey(RuntimeItemKey))
        {
            return;
        }

        items[RuntimeItemKey] = true;
        AppendRuntime(builder);
    }

    public static void Append(StringBuilder builder, IEnumerable<ClientAsset> assets)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(assets);

        foreach (var asset in assets)
        {
            if (asset.Kind == ClientAssetKind.Style)
            {
                builder.Append("<link rel=\"stylesheet\" href=\"").Append(Encode(asset.Url)).Append("\" />");
            }
            else
            {
                AppendScript(builder, asset.Url);
            }
        }
    }

    public static void AppendProvider(StringBuilder builder, string elementId, string configuration, string scriptUrl)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(elementId);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrEmpty(scriptUrl);

        Append(builder, [
            ClientInclude.Json(elementId, configuration),
            ClientInclude.Script(scriptUrl)
        ]);
    }

    public static void Append(StringBuilder builder, IEnumerable<ClientInclude> includes)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(includes);

        foreach (var include in includes)
        {
            switch (include.Kind)
            {
                case "style":
                    builder.Append("<link rel=\"stylesheet\" href=\"").Append(Encode(include.Url ?? "")).Append("\" />");
                    break;
                case "script":
                    AppendScript(builder, include.Url ?? "");
                    break;
                case "json":
                    builder.Append("<script type=\"application/json\" id=\"").Append(Encode(include.Id ?? "")).Append("\">");
                    builder.Append(include.Content);
                    builder.Append("</script>");
                    break;
            }
        }
    }

    private static void AppendScript(StringBuilder builder, string url) =>
        builder.Append("<script src=\"").Append(Encode(url)).Append("\"></script>");

    private static string Encode(string value) => WebUtility.HtmlEncode(value);
}