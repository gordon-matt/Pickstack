using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.RichTextEditor.TinyMce;

/// <summary>
/// Represents the tiny mce editor.
/// </summary>
public sealed class TinyMceEditor : IRichTextEditor, IPickstackAssetSource
{
    /// <summary>
    /// The key constant.
    /// </summary>
    public const string Key = "TinyMCE";

    private readonly TinyMceOptions _options;

    /// <summary>
    /// Initializes a new instance of the TinyMceEditor class.
    /// </summary>
    /// <param name="options">The options.</param>
    public TinyMceEditor(IOptions<TinyMceOptions> options)
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
    public string ProviderScriptUrl => "/pickstack/assets/tinymce.js";

    /// <summary>
    /// The provider script loads TinyMCE itself so a CDN failure can fall back to the local copy
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
        init = _options.CreateInit()
    });

    /// <summary>
    /// Gets the assets.
    /// </summary>
    /// <returns>The assets.</returns>
    public IEnumerable<PickstackAsset> GetAssets() =>
    [
        PickstackAssets.Script(typeof(TinyMceEditor).Assembly, "tinymce.js", "tinymce.js")
    ];
}