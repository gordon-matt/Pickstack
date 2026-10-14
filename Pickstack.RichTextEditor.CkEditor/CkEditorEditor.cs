using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.RichTextEditor.CkEditor;

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

    public string GetProviderConfiguration() => JsonSerializer.Serialize(_options.CreateConfig());

    public IEnumerable<PickstackAsset> GetAssets() =>
    [
        PickstackAssets.Script(typeof(CkEditorEditor).Assembly, "ckeditor.js", "ckeditor.js")
    ];
}