using System.Net;
using System.Text;

namespace Pickstack.Abstractions;

/// <summary>
/// Shared markup for the Pickstack browser runtime and a component's client assets.
/// </summary>
public static class ClientAssets
{
    /// <summary>Address of the shared browser script. The value is <c>/pickstack/assets/pickstack.js</c>.</summary>
    public const string RuntimeScriptUrl = "/pickstack/assets/pickstack.js";

    /// <summary>Item key that records the runtime script as already written. The value is <c>Pickstack.RuntimeIncluded</c>.</summary>
    public const string RuntimeItemKey = "Pickstack.RuntimeIncluded";

    /// <summary>Appends a script tag for <see cref="RuntimeScriptUrl"/>.</summary>
    /// <param name="builder">The markup being built.</param>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <see langword="null"/>.</exception>
    public static void AppendRuntime(StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        AppendScript(builder, RuntimeScriptUrl);
    }

    /// <summary>
    /// Appends the runtime script unless <paramref name="items"/> already contains <see cref="RuntimeItemKey"/>.
    /// The key is then set, so a later call on the same dictionary writes nothing.
    /// </summary>
    /// <param name="builder">The markup being built.</param>
    /// <param name="items">The request or page items used to remember that the script was written.</param>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="items"/> is <see langword="null"/>.</exception>
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

    /// <summary>Appends a <c>link</c> for each style and a <c>script</c> for each script. Addresses are HTML-encoded.</summary>
    /// <param name="builder">The markup being built.</param>
    /// <param name="assets">The assets, in the order they should load.</param>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="assets"/> is <see langword="null"/>.</exception>
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
    /// Appends a JSON script block, then the provider script. The JSON is written as supplied, so it must already be HTML-safe.
    /// </summary>
    /// <param name="builder">The markup being built.</param>
    /// <param name="elementId">The <c>id</c> of the JSON script element.</param>
    /// <param name="configuration">The JSON the provider script reads.</param>
    /// <param name="scriptUrl">Address of the provider script.</param>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="configuration"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="elementId"/> or <paramref name="scriptUrl"/> is <see langword="null"/> or empty.</exception>
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
    /// Appends each include. <c>style</c> and <c>script</c> use the address. <c>json</c> writes <see cref="ClientInclude.Content"/> inside a script element whose <c>id</c> is <see cref="ClientInclude.Id"/>.
    /// Any other kind is skipped. Addresses and ids are HTML-encoded. JSON content is written as supplied.
    /// </summary>
    /// <param name="builder">The markup being built.</param>
    /// <param name="includes">The includes, in the order they should appear.</param>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="includes"/> is <see langword="null"/>.</exception>
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