using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.RichTextEditor.CkEditor;

/// <summary>CKEditor 4 as <see cref="IRichTextEditor"/>. Browse Server opens the registered file manager.</summary>
public sealed class CkEditorEditor : IRichTextEditor, IPickstackAssetSource
{
    /// <summary>The display name. The value is <c>CKEditor</c>.</summary>
    public const string Key = "CKEditor";

    private readonly CkEditorOptions _options;

    /// <summary>Reads the CKEditor options.</summary>
    /// <param name="options">The CKEditor options.</param>
    public CkEditorEditor(IOptions<CkEditorOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>The display name. The value is <see cref="Key"/>.</summary>
    public string Name => Key;

    /// <summary>The embedded provider script. The value is <c>/pickstack/assets/ckeditor.js</c>.</summary>
    public string ProviderScriptUrl => "/pickstack/assets/ckeditor.js";

    /// <summary>The CKEditor script from <see cref="CkEditorOptions.ScriptUrl"/>.</summary>
    /// <returns>That one script.</returns>
    public IReadOnlyList<ClientAsset> GetClientAssets() =>
    [
        new ClientAsset { Url = _options.ScriptUrl, Kind = ClientAssetKind.Script }
    ];

    /// <summary>JSON for the provider script: height, browse-window size, and any toolbar or plugin lists that were set.</summary>
    /// <returns>The JSON.</returns>
    public string GetProviderConfiguration() => JsonSerializer.Serialize(_options.CreateConfig());

    /// <summary>The embedded <c>ckeditor.js</c> provider script.</summary>
    /// <returns>The asset.</returns>
    public IEnumerable<PickstackAsset> GetAssets() =>
    [
        PickstackAssets.Script(typeof(CkEditorEditor).Assembly, "ckeditor.js", "ckeditor.js")
    ];
}