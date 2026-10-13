namespace Pickstack.FileManager;

/// <summary>
/// A replaceable file manager. Editors and file fields never reference a concrete
/// implementation; they call <c>Pickstack.files</c>, and the registered manager supplies it.
/// </summary>
public interface IFileManager
{
    /// <summary>Display name, also used when the demo chooses a manager from configuration.</summary>
    string Name { get; }

    /// <summary>Styles and scripts that must appear before <see cref="ProviderScriptUrl"/>.</summary>
    IReadOnlyList<ClientAsset> GetClientAssets();

    /// <summary>Script that calls <c>Pickstack.files.use</c>.</summary>
    string ProviderScriptUrl { get; }

    /// <summary>JSON read by the provider script. Must be produced with a serializer that escapes HTML.</summary>
    string GetProviderConfiguration();
}