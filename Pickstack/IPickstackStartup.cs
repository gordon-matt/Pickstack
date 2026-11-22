using Microsoft.AspNetCore.Builder;

namespace Pickstack;

/// <summary>
/// Middleware and endpoints contributed by an adapter. <see cref="WebApplicationExtensions.UsePickstack"/>
/// runs these in registration order.
/// </summary>
public interface IPickstackStartup
{
    /// <summary>Adds this adapter's middleware and endpoints. Called by <see cref="WebApplicationExtensions.UsePickstack"/> in registration order.</summary>
    /// <param name="app">The application being started.</param>
    void Use(WebApplication app);
}