using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pickstack.RichTextEditor;

[HtmlTargetElement("pickstack-editor", Attributes = ForAttribute, TagStructure = TagStructure.WithoutEndTag)]
public sealed class EditorTagHelper : TagHelper
{
    private const string ForAttribute = "asp-for";

    private readonly IRichTextEditor _editor;

    public EditorTagHelper(IRichTextEditor editor)
    {
        _editor = editor;
    }

    [HtmlAttributeName(ForAttribute)]
    public ModelExpression For { get; set; } = default!;

    public int? Height { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        string fieldName = For.Name;
        string fieldId = TagBuilder.CreateSanitizedId(fieldName, "_");
        string value = For.Model as string ?? string.Empty;
        string optionsJson = Height is int height
            ? JsonSerializer.Serialize(new { height })
            : "{}";

        var builder = new StringBuilder();
        EditorAssets.AppendStartup(builder, ViewContext.HttpContext, _editor);
        builder.Append("<textarea class=\"form-control\" id=\"").Append(WebUtility.HtmlEncode(fieldId));
        builder.Append("\" name=\"").Append(WebUtility.HtmlEncode(fieldName));
        builder.Append("\" rows=\"12\" data-pickstack-editor=\"true\" data-editor-options=\"");
        builder.Append(WebUtility.HtmlEncode(optionsJson));
        builder.Append("\">");
        builder.Append(WebUtility.HtmlEncode(value));
        builder.Append("</textarea>");

        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "pickstack-editor mb-3");
        output.Content.SetHtmlContent(builder.ToString());
    }
}