using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pickstack.FileManager;

[HtmlTargetElement("pickstack-file", Attributes = ForAttribute, TagStructure = TagStructure.WithoutEndTag)]
public sealed class FilePickerTagHelper : TagHelper
{
    private const string ForAttribute = "asp-for";

    private readonly IFileManager _fileManager;

    public FilePickerTagHelper(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    [HtmlAttributeName(ForAttribute)]
    public ModelExpression For { get; set; } = default!;

    [HtmlAttributeName("file-type")]
    public FilePickerType FileType { get; set; } = FilePickerType.Any;

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        string fieldName = For.Name;
        string fieldId = TagBuilder.CreateSanitizedId(fieldName, "_");
        string value = For.Model as string ?? string.Empty;
        string fileType = FileType.ToString().ToLowerInvariant();

        var builder = new StringBuilder();
        FileManagerAssets.AppendStartup(builder, ViewContext.HttpContext, _fileManager);
        builder.Append("<div class=\"input-group\">");
        builder.Append("<input class=\"form-control\" type=\"text\" id=\"").Append(Encode(fieldId));
        builder.Append("\" name=\"").Append(Encode(fieldName));
        builder.Append("\" value=\"").Append(Encode(value)).Append("\" />");
        builder.Append("<button type=\"button\" class=\"btn btn-outline-secondary\" data-pickstack-browse>Browse</button>");
        builder.Append("</div>");
        builder.Append("<div data-pickstack-preview>");
        if (FileType == FilePickerType.Image && !string.IsNullOrWhiteSpace(value))
        {
            builder.Append("<img src=\"").Append(Encode(value)).Append("\" alt=\"\" />");
        }
        builder.Append("</div>");

        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "pickstack-file mb-3");
        output.Attributes.SetAttribute("data-pickstack-file-input", "true");
        output.Attributes.SetAttribute("data-file-type", fileType);
        output.Content.SetHtmlContent(builder.ToString());
    }

    private static string Encode(string value) => System.Net.WebUtility.HtmlEncode(value);
}