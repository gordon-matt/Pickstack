namespace Pickstack.Abstractions;

/// <summary>
/// A replaceable component that contributes styles, scripts, and a provider script.
/// </summary>
public interface IPickstackComponent
{
    /// <summary>Display name, also used when an app chooses an implementation from configuration.</summary>
    string Name { get; }

    /// <summary>Styles and scripts that must appear before <see cref="ProviderScriptUrl"/>.</summary>
    IReadOnlyList<ClientAsset> GetClientAssets();

    /// <summary>Script that registers the component with the Pickstack browser runtime.</summary>
    string ProviderScriptUrl { get; }

    /// <summary>JSON read by the provider script. Must be produced with a serializer that escapes HTML.</summary>
    string GetProviderConfiguration();
}