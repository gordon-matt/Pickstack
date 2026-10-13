namespace Pickstack.RichTextEditor;

/// <summary>
/// A replaceable rich text editor. File browsing goes through <c>Pickstack.files</c>,
/// so the editor does not know which file manager is installed.
/// </summary>
public interface IRichTextEditor
{
    string Name { get; }

    IReadOnlyList<ClientAsset> GetClientAssets();

    string ProviderScriptUrl { get; }

    /// <summary>JSON read by the provider script. Must be produced with a serializer that escapes HTML.</summary>
    string GetProviderConfiguration();
}