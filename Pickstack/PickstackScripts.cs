using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Pickstack;

/// <summary>
/// Loads the shared Pickstack runtime and the scripts for the registered file manager and rich text editor.
/// Interactive Blazor does not run script tags emitted by a component, so those hosts inject the same list through the browser.
/// </summary>
public sealed class PickstackScripts : ComponentBase
{
    private bool _started;

    [Inject]
    public IServiceProvider Services { get; set; } = default!;

    [Inject]
    public IJSRuntime Js { get; set; } = default!;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (RendererInfo.IsInteractive)
        {
            return;
        }

        builder.AddMarkupContent(0, PickstackPageScripts.Html(Services));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || !RendererInfo.IsInteractive || _started)
        {
            return;
        }

        _started = true;
        var includes = PickstackPageScripts.Resolve(Services);
        if (includes.Count == 0)
        {
            return;
        }

        // The loader is only needed for this one call, so release it straight away. Holding it until the
        // component is disposed would mean a JS call while the circuit is being torn down, which always fails.
        await using var module = await Js.InvokeAsync<IJSObjectReference>("import", "/pickstack/assets/load.js");
        await module.InvokeVoidAsync("load", includes);
    }
}
