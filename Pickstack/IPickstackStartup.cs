using Microsoft.AspNetCore.Builder;

namespace Pickstack;

/// <summary>
/// Middleware and endpoints contributed by an adapter. <see cref="WebApplicationExtensions.UsePickstack"/>
/// runs these in registration order.
/// </summary>
public interface IPickstackStartup
{
    /// <summary>
    /// Uses the value.
    /// </summary>
    /// <param name="app">The app.</param>
    void Use(WebApplication app);
}