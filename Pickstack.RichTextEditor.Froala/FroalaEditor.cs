using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.RichTextEditor.Froala;

/// <summary>Froala as <see cref="IRichTextEditor"/>. The provider script loads Froala so a CDN failure can use the local copy.</summary>
public sealed class FroalaEditor : IRichTextEditor, IPickstackAssetSource
{
    /// <summary>The name used when <see cref="FroalaOptions.Name"/> is blank. The value is <c>Froala</c>.</summary>
    public const string Key = "Froala";

    private readonly FroalaOptions _options;

    /// <summary>Reads the Froala options.</summary>
    /// <param name="options">The Froala options.</param>
    public FroalaEditor(IOptions<FroalaOptions> options)
    {
        _options = options.Value;
    }

    /// <summary><see cref="FroalaOptions.Name"/>, or <see cref="Key"/> when that name is blank.</summary>
    public string Name => string.IsNullOrWhiteSpace(_options.Name) ? Key : _options.Name;

    /// <summary>The embedded provider script. The value is <c>/pickstack/assets/froala.js</c>.</summary>
    public string ProviderScriptUrl => "/pickstack/assets/froala.js";

    /// <summary>
    /// No styles or scripts. The provider script loads Froala itself so a CDN failure can fall back to the local copy
    /// before any editor is initialized.
    /// </summary>
    /// <returns>An empty list.</returns>
    public IReadOnlyList<ClientAsset> GetClientAssets() => [];

    /// <summary>JSON for the provider script: the script and stylesheet addresses, their fallbacks, and the init object.</summary>
    /// <returns>The JSON.</returns>
    public string GetProviderConfiguration() => JsonSerializer.Serialize(new
    {
        scriptUrl = _options.ScriptUrl,
        fallbackUrl = _options.FallbackUrl,
        styleUrl = _options.StyleUrl,
        fallbackStyleUrl = _options.FallbackStyleUrl,
        init = _options.CreateInit()
    });

    /// <summary>The embedded <c>froala.js</c> provider script.</summary>
    /// <returns>The asset.</returns>
    public IEnumerable<PickstackAsset> GetAssets() =>
    [
        PickstackAssets.Script(typeof(FroalaEditor).Assembly, "froala.js", "froala.js")
    ];
}