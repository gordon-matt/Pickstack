using System.Text.Json;
using Microsoft.Extensions.Options;
using Pickstack.Abstractions;

namespace Pickstack.RichTextEditor.CkEditor;

/// <summary>
/// Represents the ck editor editor.
/// </summary>
public sealed class CkEditorEditor : IRichTextEditor, IPickstackAssetSource
{
    /// <summary>
    /// The key constant.
    /// </summary>
    public const string Key = "CKEditor";

    private readonly CkEditorOptions _options;

    /// <summary>
    /// Initializes a new instance of the CkEditorEditor class.
    /// </summary>
    /// <param name="options">The options.</param>
    public CkEditorEditor(IOptions<CkEditorOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name => Key;

    /// <summary>
    /// Gets the provider script URL.
    /// </summary>
    public string ProviderScriptUrl => "/pickstack/assets/ckeditor.js";

    /// <summary>
    /// Gets the client assets.
    /// </summary>
    /// <returns>The client assets.</returns>
    public IReadOnlyList<ClientAsset> GetClientAssets() =>
    [
        new ClientAsset { Url = _options.ScriptUrl, Kind = ClientAssetKind.Script }
    ];

    /// <summary>
    /// Gets the provider configuration.
    /// </summary>
    /// <returns>The provider configuration.</returns>
    public string GetProviderConfiguration() => JsonSerializer.Serialize(_options.CreateConfig());

    /// <summary>
    /// Gets the assets.
    /// </summary>
    /// <returns>The assets.</returns>
    public IEnumerable<PickstackAsset> GetAssets() =>
    [
        PickstackAssets.Script(typeof(CkEditorEditor).Assembly, "ckeditor.js", "ckeditor.js")
    ];
}