using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.RichTextEditor.Froala;

/// <summary>
/// Represents the froala editor.
/// </summary>
public sealed class FroalaEditor : IRichTextEditor, IPickstackAssetSource
{
    /// <summary>
    /// The key constant.
    /// </summary>
    public const string Key = "Froala";

    private readonly FroalaOptions _options;

    /// <summary>
    /// Initializes a new instance of the FroalaEditor class.
    /// </summary>
    /// <param name="options">The options.</param>
    public FroalaEditor(IOptions<FroalaOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name => string.IsNullOrWhiteSpace(_options.Name) ? Key : _options.Name;

    /// <summary>
    /// Gets the provider script URL.
    /// </summary>
    public string ProviderScriptUrl => "/pickstack/assets/froala.js";

    /// <summary>
    /// The provider script loads Froala itself so a CDN failure can fall back to the local copy
    /// before any editor is initialized.
    /// </summary>
    public IReadOnlyList<ClientAsset> GetClientAssets() => [];

    /// <summary>
    /// Gets the provider configuration.
    /// </summary>
    /// <returns>The provider configuration.</returns>
    public string GetProviderConfiguration() => JsonSerializer.Serialize(new
    {
        scriptUrl = _options.ScriptUrl,
        fallbackUrl = _options.FallbackUrl,
        styleUrl = _options.StyleUrl,
        fallbackStyleUrl = _options.FallbackStyleUrl,
        init = _options.CreateInit()
    });

    /// <summary>
    /// Gets the assets.
    /// </summary>
    /// <returns>The assets.</returns>
    public IEnumerable<PickstackAsset> GetAssets() =>
    [
        PickstackAssets.Script(typeof(FroalaEditor).Assembly, "froala.js", "froala.js")
    ];
}