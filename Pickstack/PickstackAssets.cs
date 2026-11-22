using System.Reflection;

namespace Pickstack;

/// <summary>Builds an embedded JavaScript asset served from <c>/pickstack/assets/{name}</c>.</summary>
public static class PickstackAssets
{
    /// <summary>A JavaScript asset whose body is an embedded resource.</summary>
    /// <param name="assembly">The assembly that contains the resource.</param>
    /// <param name="resourceName">The manifest resource name.</param>
    /// <param name="name">The name used in <c>/pickstack/assets/{name}</c>. Matching ignores case.</param>
    /// <returns>The asset. Opening it throws <see cref="InvalidOperationException"/> when the resource is missing.</returns>
    public static PickstackAsset Script(Assembly assembly, string resourceName, string name) => new()
    {
        Name = name,
        ContentType = "application/javascript",
        OpenRead = () => assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded asset '{resourceName}' was not found in {assembly.GetName().Name}.")
    };
}