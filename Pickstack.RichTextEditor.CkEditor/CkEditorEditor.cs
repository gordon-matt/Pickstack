using Microsoft.Extensions.Options;
using Pickstack;
using Pickstack.RichTextEditor;

namespace Pickstack.RichTextEditor.CkEditor;

public sealed class CkEditorOptions
{
    public string ScriptUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/ckeditor/4.17.1/ckeditor.js";
}

public sealed class CkEditorEditor : IRichTextEditor, IPickstackAssetSource
{
    public const string Key = "CKEditor";

    private readonly CkEditorOptions _options;

    public CkEditorEditor(IOptions<CkEditorOptions> options)
    {
        _options = options.Value;
    }

    public string Name => Key;

    public string ProviderScriptUrl => "/pickstack/assets/ckeditor.js";

    public IReadOnlyList<ClientAsset> GetClientAssets() =>
    [
        new ClientAsset { Url = _options.ScriptUrl, Kind = ClientAssetKind.Script }
    ];

    public IEnumerable<PickstackAsset> GetAssets() =>
    [
        PickstackAssets.Script(typeof(CkEditorEditor).Assembly, "ckeditor.js", "ckeditor.js")
    ];
}
