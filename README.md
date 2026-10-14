<img src="https://github.com/gordon-matt/Pickstack/blob/master/_Misc/Logo.png" alt="Logo" width="300" />

Pickstack is a set of replaceable ASP.NET Core components. An app picks a file manager and a rich text editor, and can later swap either one by changing a package reference and a registration. File fields and editors keep working because they talk to Pickstack, not to elFinder, Responsive File Manager, TinyMCE, or CKEditor.

## How a swap stays invisible

A page renders two tags:

```html
<pickstack-file asp-for="CoverImage" file-type="Image" />
<pickstack-editor asp-for="Body" />
```

Those tag helpers ask dependency injection for `IFileManager` and `IRichTextEditor`. The registered adapter emits its own styles, scripts, and a small provider script.

Every file manager provider implements the same browser function:

```javascript
Pickstack.files.open({ type: "image", callback: function (file) { /* file.url */ } });
```

The cover-image button calls that. TinyMCE's file picker calls that. CKEditor opens a Pickstack browse page that mounts the same manager. Replacing the manager replaces the function, so every picker on the site follows.

Both adapters read and write `wwwroot/media` (URL `/media/`). Files uploaded with one manager are still there after the swap.

```
Pickstack.Abstractions                        shared client assets and component contract
Pickstack.FileManager                         contract, storage, tag helpers
  Pickstack.FileManager.elFinder              elFinder.NetCore
  Pickstack.FileManager.ResponsiveFileManager Responsive File Manager
Pickstack.RichTextEditor                      contract and tag helper
  Pickstack.RichTextEditor.TinyMce
  Pickstack.RichTextEditor.CkEditor           browse page that hosts IFileManager
Pickstack                                     composition, shared browser runtime
```

An editor package does not reference an elFinder or Responsive File Manager package. It only uses `Pickstack.files`.

## Demo

The demo references every adapter so you can switch without changing project references. In `Pickstack.Demo/appsettings.json`:

```json
"Pickstack": {
  "FileManager": "elFinder",
  "RichTextEditor": "TinyMCE"
}
```

`FileManager` is `elFinder` or `ResponsiveFileManager`. `RichTextEditor` is `TinyMCE` or `CKEditor`. Restart the app after changing them.

`Pickstack:Storage` is the shared upload folder. `WebRootFolder` can be nested, such as `Media/Uploads`. Keep the thumbnail folder outside the upload folder; Responsive File Manager requires that.

Responsive File Manager's NuGet package targets .NET 9. The adapter project sets `AssetTargetFallback` so this .NET 10 solution can reference it.

## Use it in an app

Reference `Pickstack`, the file-manager contract, the editor contract, and one adapter for each. Then register them:

```csharp
builder.Services.AddControllersWithViews();

builder.Services.AddPickstack(builder.Configuration)
    .AddElFinder()
    .AddTinyMce();

var app = builder.Build();

app.UseStaticFiles();
app.UsePickstack();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

`UseStaticFiles()` is what serves files saved under `wwwroot/media`. `UsePickstack()` creates those folders and serves the shared browser script. The Responsive File Manager adapter also starts its session and PHP middleware there, so the host should not call `UseSession` itself.

Add the tag helpers in `_ViewImports.cshtml`:

```cshtml
@addTagHelper *, Pickstack.FileManager
@addTagHelper *, Pickstack.RichTextEditor
```

elFinder's browser needs jQuery on the page before the picker scripts. The demo loads it in the layout head. The CKEditor browse window loads its own copy.

To swap elFinder for Responsive File Manager, change the package reference and `.AddElFinder()` to `.AddResponsiveFileManager()`. Leave the views alone.

## Configure an adapter

Each `Add` method accepts a fluent options object for that product. These settings apply to every instance. A field can override them:

```html
<pickstack-file asp-for="CoverImage" file-type="Image" file-root="42/7" />
<pickstack-files file-type="Folder" file-root="42/7" />
<pickstack-editor asp-for="Body" file-root="42/7" add-plugins="contentzone" toolbar="undo redo | contentzone" />
```

`file-root` is a relative folder under the upload directory. `42/7` is a tenant and user; a single segment works the same way. `file-type="Folder"` shows directories. A `<pickstack-editor>` `height`, `plugins`, `add-plugins`, or `toolbar` attribute applies to that field only. The file picker stays connected to Pickstack.

Same-origin file URLs are stored as root-relative paths, such as `/media/42/7/photo.png`. TinyMCE keeps that form by removing the script host.

```csharp
builder.Services.AddPickstack(builder.Configuration)
    .AddElFinder(files => files
        .DisableCommands("extract", "archive")
        .AddToolbarGroup("fullscreen"))
    .AddTinyMce(editor => editor
        .AddPlugins("codesample", "charmap")
        .WithToolbar("undo redo | bold italic | codesample charmap | link image")
        .Set("branding", false));
```

`Set` passes a value through to the underlying init object. Named settings such as `WithToolbar` are applied after it.

CKEditor groups buttons the same way. An empty toolbar leaves CKEditor's default toolbar in place.

```csharp
.AddCkEditor(editor => editor
    .WithoutPlugins("elementspath")
    .WithToolbar(
        ["Bold", "Italic", "Underline"],
        ["Link", "Image"]));
```

Responsive File Manager's dialog options are fluent too. `Server` configures the PHP package, including upload limits:

```csharp
.AddResponsiveFileManager(files => files
    .WithLanguage("en_EN")
    .Server(server => server.MaxSizeUpload = 16));
```

Call `.UseHostSession()` when the host already calls `UseSession()`, as Mantle does. Pickstack then leaves that session in place.


## Add another implementation

A file manager implements `IFileManager`:

- `GetClientAssets()` returns the styles and scripts it needs.
- `ProviderScriptUrl` points at a script, usually embedded and served from `/pickstack/assets/{name}`.
- `GetProviderConfiguration()` returns JSON for that script. Use `JsonSerializer` so `<` is escaped.
- The script calls `Pickstack.files.use({ mount: function (element, options) { ... } })`.
- `options.select` means the user is choosing a file. Call `Pickstack.files.notify({ url, name })` when they do.
- `options.type` is `any`, `file`, `image`, `media`, or `folder`.
- `options.root` is the relative folder to open, such as `42/7`. Pickstack creates it before calling `mount`.
- Register the class as `IFileManager` and as `IPickstackAssetSource` if the script is embedded. `AddFileManager<T>()` only registers the interface; an adapter with endpoints or middleware should add its own extension, as `AddElFinder` does.

An editor implements `IRichTextEditor` and registers with `Pickstack.editors.use({ init: function (element, options) { ... } })`. When it needs a file, it calls `Pickstack.files.open` instead of embedding a file-manager URL.
