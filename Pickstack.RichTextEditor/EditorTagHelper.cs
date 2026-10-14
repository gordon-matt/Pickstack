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

    /// <summary>Replaces the registered plugin list for this field.</summary>
    [HtmlAttributeName("plugins")]
    public string? Plugins { get; set; }

    /// <summary>Plugins added to the registered list for this field.</summary>
    [HtmlAttributeName("add-plugins")]
    public string? AddPlugins { get; set; }

    /// <summary>Replaces the registered toolbar for this field.</summary>
    [HtmlAttributeName("toolbar")]
    public string? Toolbar { get; set; }

    /// <summary>Relative upload folder opened by this field's file picker, such as <c>42/7</c>.</summary>
    [HtmlAttributeName("file-root")]
    public string? FileRoot { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        string fieldName = For.Name;
        string fieldId = TagBuilder.CreateSanitizedId(fieldName, "_");
        string value = For.Model as string ?? string.Empty;
        string optionsJson = JsonSerializer.Serialize(InstanceOptions());

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

    private Dictionary<string, object?> InstanceOptions()
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