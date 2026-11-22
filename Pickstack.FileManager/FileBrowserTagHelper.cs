using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pickstack.FileManager;

/// <summary>
/// A <c>pickstack-files</c> browser. It opens the registered file manager without writing a chosen file into a field.
/// <c>data-select</c> is false.
/// </summary>
[HtmlTargetElement("pickstack-files", TagStructure = TagStructure.WithoutEndTag)]
public sealed class FileBrowserTagHelper : TagHelper
{
    private readonly IFileManager _fileManager;

    /// <summary>Uses the registered file manager for the startup scripts written into the browser.</summary>
    /// <param name="fileManager">The registered file manager.</param>
    public FileBrowserTagHelper(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    /// <summary>What the browser offers. The attribute is <c>file-type</c>. The default is <see cref="FilePickerType.Any"/>.</summary>
    [HtmlAttributeName("file-type")]
    public FilePickerType FileType { get; set; } = FilePickerType.Any;

    /// <summary>Relative folder under the upload directory, such as <c>42/7</c>.</summary>
    [HtmlAttributeName("file-root")]
    public string? FileRoot { get; set; }

    /// <summary>The current view. The startup scripts are written once for this request.</summary>
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        var builder = new StringBuilder();
        FileManagerAssets.AppendStartup(builder, ViewContext.HttpContext, _fileManager);

        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "pickstack-files");
        output.Attributes.SetAttribute("data-pickstack-browser", "true");
        output.Attributes.SetAttribute("data-select", "false");
        output.Attributes.SetAttribute("data-file-type", FileType.ToString().ToLowerInvariant());
        if (!string.IsNullOrWhiteSpace(FileRoot))
        {
            output.Attributes.SetAttribute("data-file-root", FileRoot);
        }
        output.Content.SetHtmlContent(builder.ToString());
    }
}