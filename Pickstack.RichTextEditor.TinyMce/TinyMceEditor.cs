using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.RichTextEditor.TinyMce;

public sealed class TinyMceEditor : IRichTextEditor, IPickstackAssetSource
{
    public const string Key = "TinyMCE";

    private readonly TinyMceOptions _options;

    public TinyMceEditor(IOptions<TinyMceOptions> options)
    {
        _options = options.Value;
    }

    public string Name => string.IsNullOrWhiteSpace(_options.Name) ? Key : _options.Name;

    public string ProviderScriptUrl => "/pickstack/assets/tinymce.js";

    /// <summary>
    /// The provider script loads TinyMCE itself so a CDN failure can fall back to the local copy
    /// before any editor is initialized.
    /// </summary>
    public IReadOnlyList<ClientAsset> GetClientAssets() => [];

    public string GetProviderConfiguration() => JsonSerializer.Serialize(new
    {
        scriptUrl = _options.ScriptUrl,
        fallbackUrl = _options.FallbackUrl,
        init = _options.CreateInit()
    });

    public IEnumerable<PickstackAsset> GetAssets() =>
    [
        PickstackAssets.Script(typeof(TinyMceEditor).Assembly, "tinymce.js", "tinymce.js")
    ];
}