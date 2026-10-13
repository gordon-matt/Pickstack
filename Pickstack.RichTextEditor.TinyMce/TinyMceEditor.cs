using Microsoft.Extensions.Options;
using Pickstack;
using Pickstack.RichTextEditor;

namespace Pickstack.RichTextEditor.TinyMce;

public sealed class TinyMceOptions
{
    public string ScriptUrl { get; set; } = "https://cdn.jsdelivr.net/npm/tinymce@7.9.0/tinymce.min.js";
}

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

    public IEnumerable<PickstackAsset> GetAssets() =>
    [
        PickstackAssets.Script(typeof(TinyMceEditor).Assembly, "tinymce.js", "tinymce.js")
    ];
}
