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
    private string? _pendingValue;
    private int _pendingVersion;
    private int _sentVersion;
    private bool _sending;
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
        await SendPendingAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_fromEditor)
        {
            _fromEditor = false;
            return;
        }

        string value = Value ?? string.Empty;

        // The first render writes the textarea the editor reads as it starts. Until that element
        // exists, keep the starting value current. Afterwards a change has to be sent explicitly,
        // and one that arrives while the script is loading is held until mount returns.
        if (!_hasElement)
        {
            _initialValue = value;
            return;
        }

        if (!_mounted || _module is null)
        {
            if (!string.Equals(value, _initialValue, StringComparison.Ordinal))
            {
                Hold(value);
            }

            return;
        }

        Hold(value);
        await SendPendingAsync();
    }

    private void Hold(string value)
    {
        if (_pendingVersion > 0 && string.Equals(value, _pendingValue, StringComparison.Ordinal))
        {
            return;
        }

        _pendingValue = value;
        _pendingVersion++;
    }

    /// <summary>
    /// One send at a time. A value that arrives while a send is in flight waits for it, so an older
    /// value cannot land after a newer one.
    /// </summary>
    private async Task SendPendingAsync()
    {
        if (_sending || _module is null)
        {
            return;
        }

        _sending = true;
        try
        {
            while (_module is not null && _sentVersion != _pendingVersion && _pendingValue is string pending)
            {
                int version = _pendingVersion;
                await _module.InvokeVoidAsync("setContent", _element, pending);
                if (version == _pendingVersion)
                {
                    _sentVersion = version;
                }
            }
        }
        finally
        {
            _sending = false;
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
