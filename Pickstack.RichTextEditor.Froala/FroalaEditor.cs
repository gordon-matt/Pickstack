using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.RichTextEditor.Froala;

public sealed class FroalaEditor : IRichTextEditor, IPickstackAssetSource
{
    public const string Key = "Froala";

    private readonly FroalaOptions _options;

    public FroalaEditor(IOptions<FroalaOptions> options)
    {
        _options = options.Value;
    }

    public string Name => string.IsNullOrWhiteSpace(_options.Name) ? Key : _options.Name;

    public string ProviderScriptUrl => "/pickstack/assets/froala.js";

    /// <summary>
    /// The provider script loads Froala itself so a CDN failure can fall back to the local copy
    /// before any editor is initialized.
    /// </summary>
    public IReadOnlyList<ClientAsset> GetClientAssets() => [];

    public string GetProviderConfiguration() => JsonSerializer.Serialize(new
    {
        scriptUrl = _options.ScriptUrl,
        fallbackUrl = _options.FallbackUrl,
        styleUrl = _options.StyleUrl,
        fallbackStyleUrl = _options.FallbackStyleUrl,
        init = _options.CreateInit()
    });

    public IEnumerable<PickstackAsset> GetAssets() =>
    [
        PickstackAssets.Script(typeof(FroalaEditor).Assembly, "froala.js", "froala.js")
    ];
}