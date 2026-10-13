using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack;

namespace Pickstack.RichTextEditor.TinyMce;

public sealed class TinyMceEditor : IRichTextEditor, IPickstackAssetSource
{
    public const string Key = "TinyMCE";

    private readonly TinyMceOptions _options;

    public TinyMceEditor(IOptions<TinyMceOptions> options)
    {
        _options = options.Value;
    }

    public string Name => Key;

    public string ProviderScriptUrl => "/pickstack/assets/tinymce.js";

    public IReadOnlyList<ClientAsset> GetClientAssets() =>
    [
        new ClientAsset { Url = _options.ScriptUrl, Kind = ClientAssetKind.Script }
    ];

    public string GetProviderConfiguration() => JsonSerializer.Serialize(_options.CreateInit());

    public IEnumerable<PickstackAsset> GetAssets() =>
    [
        PickstackAssets.Script(typeof(TinyMceEditor).Assembly, "tinymce.js", "tinymce.js")
    ];
}
