using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pickstack.FileManager;

[HtmlTargetElement("pickstack-files", TagStructure = TagStructure.WithoutEndTag)]
public sealed class FileBrowserTagHelper : TagHelper
{
    private readonly IFileManager _fileManager;

    public FileBrowserTagHelper(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    [HtmlAttributeName("file-type")]
    public FilePickerType FileType { get; set; } = FilePickerType.Any;

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

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
        output.Content.SetHtmlContent(builder.ToString());
    }
}