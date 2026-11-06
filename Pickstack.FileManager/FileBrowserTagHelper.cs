using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pickstack.FileManager;

/// <summary>
/// Represents the file browser tag helper.
/// </summary>
[HtmlTargetElement("pickstack-files", TagStructure = TagStructure.WithoutEndTag)]
public sealed class FileBrowserTagHelper : TagHelper
{
    private readonly IFileManager _fileManager;

    /// <summary>
    /// Initializes a new instance of the FileBrowserTagHelper class.
    /// </summary>
    /// <param name="fileManager">The file manager.</param>
    public FileBrowserTagHelper(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    /// <summary>
    /// Gets or sets the file type.
    /// </summary>
    [HtmlAttributeName("file-type")]
    public FilePickerType FileType { get; set; } = FilePickerType.Any;

    /// <summary>Relative folder under the upload directory, such as <c>42/7</c>.</summary>
    [HtmlAttributeName("file-root")]
    public string? FileRoot { get; set; }

    /// <summary>
    /// Gets or sets the view context.
    /// </summary>
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