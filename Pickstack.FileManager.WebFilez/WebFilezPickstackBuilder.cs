using Microsoft.Extensions.DependencyInjection;
using WebFilez;

namespace Pickstack.FileManager.WebFilez;

/// <summary>
/// The chain after <c>AddWebFilez</c>. WebFilez package methods continue here, and
/// <see cref="Pickstack"/> returns to the product-agnostic builder.
/// </summary>
public sealed class WebFilezPickstackBuilder : IWebFilezBuilder
{
    /// <summary>Wraps the Pickstack builder that selected WebFilez.</summary>
    /// <param name="pickstack">The builder returned by <c>AddPickstack</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="pickstack"/> is <see langword="null"/>.</exception>
    public WebFilezPickstackBuilder(PickstackBuilder pickstack)
    {
        ArgumentNullException.ThrowIfNull(pickstack);
        Pickstack = pickstack;
    }

    /// <summary>The Pickstack builder, for an editor or another file manager after the WebFilez packages.</summary>
    public PickstackBuilder Pickstack { get; }

    /// <summary>The collection the WebFilez package methods write to.</summary>
    public IServiceCollection Services => Pickstack.Services;
}
