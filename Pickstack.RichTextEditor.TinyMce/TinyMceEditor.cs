using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.RichTextEditor.TinyMce;

/// <summary>TinyMCE as <see cref="IRichTextEditor"/>. The provider script loads TinyMCE so a CDN failure can use the local copy.</summary>
public sealed class TinyMceEditor : IRichTextEditor, IPickstackAssetSource
{
    /// <summary>The name used when <see cref="TinyMceOptions.Name"/> is blank. The value is <c>TinyMCE</c>.</summary>
    public const string Key = "TinyMCE";

    private readonly TinyMceOptions _options;

    /// <summary>Reads the TinyMCE options.</summary>
    /// <param name="options">The TinyMCE options.</param>
    public TinyMceEditor(IOptions<TinyMceOptions> options)
    {
        _options = options.Value;
    }

    /// <summary><see cref="TinyMceOptions.Name"/>, or <see cref="Key"/> when that name is blank.</summary>
    public string Name => string.IsNullOrWhiteSpace(_options.Name) ? Key : _options.Name;

    /// <summary>The embedded provider script. The value is <c>/pickstack/assets/tinymce.js</c>.</summary>
    public string ProviderScriptUrl => "/pickstack/assets/tinymce.js";

    /// <summary>
    /// No styles or scripts. The provider script loads TinyMCE itself so a CDN failure can fall back to the local copy
    /// before any editor is initialized.
    /// </summary>
    /// <returns>An empty list.</returns>
    public IReadOnlyList<ClientAsset> GetClientAssets() => [];

    /// <summary>JSON for the provider script: the script address, the fallback address, and the init object.</summary>
    /// <returns>The JSON.</returns>
    public string GetProviderConfiguration() => JsonSerializer.Serialize(new
    {
        scriptUrl = _options.ScriptUrl,
        fallbackUrl = _options.FallbackUrl,
        init = _options.CreateInit()
    });

    /// <summary>The embedded <c>tinymce.js</c> provider script.</summary>
    /// <returns>The asset.</returns>
    public IEnumerable<PickstackAsset> GetAssets() =>
    [
        PickstackAssets.Script(typeof(TinyMceEditor).Assembly, "tinymce.js", "tinymce.js")
    ];
}