using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Pickstack;

/// <summary>
/// Rich text editor for Blazor. The registered <c>IRichTextEditor</c> supplies the product.
/// <see cref="PickstackScripts"/> must be on the page so the provider script has loaded.
/// </summary>
public sealed class PickstackEditor : ComponentBase, IAsyncDisposable
{
    private string _initialValue = string.Empty;
    private ElementReference _element;
    private bool _hasElement;
    private bool _mounted;
    private bool _fromEditor;
    private IJSObjectReference? _module;
    private DotNetObjectReference<PickstackEditor>? _reference;

    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    [Parameter]
    public int? Height { get; set; }

    [Parameter]
    public string? Plugins { get; set; }

    [Parameter]
    public string? AddPlugins { get; set; }

    [Parameter]
    public string? Toolbar { get; set; }

    /// <summary>Relative upload folder opened by this editor's file picker, such as <c>Tenant_1</c>.</summary>
    [Parameter]
    public string? FileRoot { get; set; }

    [Inject]
    public IJSRuntime Js { get; set; } = default!;

    protected override void OnInitialized()
    {
        _initialValue = Value ?? string.Empty;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "textarea");
        builder.AddAttribute(1, "class", "pickstack-editor");
        builder.AddAttribute(2, "style", "width:100%");
        builder.AddContent(3, _initialValue);
        builder.AddElementReferenceCapture(4, element =>
        {
            _element = element;
            _hasElement = true;
        });
        builder.CloseElement();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || !_hasElement)
        {
            return;
        }

        _reference = DotNetObjectReference.Create(this);
        _module = await Js.InvokeAsync<IJSObjectReference>("import", "/pickstack/assets/editor.js");
        await _module.InvokeVoidAsync("mount", _element, Options(), _reference);
        _mounted = true;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_fromEditor)
        {
            _fromEditor = false;
            return;
        }

        if (_mounted && _module is not null && _hasElement)
        {
            await _module.InvokeVoidAsync("setContent", _element, Value ?? string.Empty);
        }
    }

    [JSInvokable]
    public async Task UpdateValue(string? value)
    {
        if (string.Equals(value, Value, StringComparison.Ordinal))
        {
            return;
        }

        _fromEditor = true;
        await ValueChanged.InvokeAsync(value);
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null && _hasElement)
        {
            try
            {
                await _module.InvokeVoidAsync("destroy", _element);
            }
            catch (JSDisconnectedException)
            {
            }

            await _module.DisposeAsync();
        }

        _reference?.Dispose();
    }

    private Dictionary<string, object?> Options()
    {
        var options = new Dictionary<string, object?>();
        if (Height is int height)
        {
            options["height"] = height;
        }

        if (!string.IsNullOrWhiteSpace(Plugins))
        {
            options["plugins"] = Plugins;
        }

        if (!string.IsNullOrWhiteSpace(AddPlugins))
        {
            options["addPlugins"] = AddPlugins;
        }

        if (!string.IsNullOrWhiteSpace(Toolbar))
        {
            options["toolbar"] = Toolbar;
        }

        if (!string.IsNullOrWhiteSpace(FileRoot))
        {
            options["fileRoot"] = FileRoot;
        }

        return options;
    }
}
