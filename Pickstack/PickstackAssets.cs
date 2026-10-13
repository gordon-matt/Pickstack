using System.Reflection;

namespace Pickstack;

public static class PickstackAssets
{
    public static PickstackAsset Script(Assembly assembly, string resourceName, string name) =>
        new()
        {
            Name = name,
            ContentType = "application/javascript",
            OpenRead = () => assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Embedded asset '{resourceName}' was not found in {assembly.GetName().Name}.")
        };
}
