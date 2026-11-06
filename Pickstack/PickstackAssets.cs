using System.Reflection;

namespace Pickstack;

/// <summary>
/// Provides pickstack assets.
/// </summary>
public static class PickstackAssets
{
    /// <summary>
    /// Gets the script.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="name">The name.</param>
    /// <returns>The script.</returns>
    public static PickstackAsset Script(Assembly assembly, string resourceName, string name) => new()
    {
        Name = name,
        ContentType = "application/javascript",
        OpenRead = () => assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded asset '{resourceName}' was not found in {assembly.GetName().Name}.")
    };
}