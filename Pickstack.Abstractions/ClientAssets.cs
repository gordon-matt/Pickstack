using System.Net;
using System.Text;

namespace Pickstack.Abstractions;

/// <summary>
/// Shared markup for the Pickstack browser runtime and a component's client assets.
/// </summary>
public static class ClientAssets
{
    /// <summary>
    /// The runtime script URL constant.
    /// </summary>
    public const string RuntimeScriptUrl = "/pickstack/assets/pickstack.js";

    /// <summary>
    /// The runtime item key constant.
    /// </summary>
    public const string RuntimeItemKey = "Pickstack.RuntimeIncluded";

    /// <summary>
    /// Appends the runtime.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void AppendRuntime(StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        AppendScript(builder, RuntimeScriptUrl);
    }

    /// <summary>
    /// Appends the runtime once.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="items">The items.</param>
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

    /// <summary>
    /// Appends the value.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assets">The assets.</param>
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

    /// <summary>
    /// Appends the provider.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="elementId">The element identifier.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="scriptUrl">The script URL.</param>
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

    /// <summary>
    /// Appends the value.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="includes">The includes.</param>
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