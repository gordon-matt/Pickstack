using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pickstack.RichTextEditor;

/// <summary>
/// A <c>pickstack-editor</c> field. It renders a textarea the registered editor replaces.
/// Field attributes override the registered height, plugins, and toolbar for this field only.
/// </summary>
[HtmlTargetElement("pickstack-editor", Attributes = ForAttribute, TagStructure = TagStructure.WithoutEndTag)]
public sealed class EditorTagHelper : TagHelper
{
    private const string ForAttribute = "asp-for";

    private readonly IRichTextEditor _editor;

    /// <summary>Uses the registered editor for the startup scripts written into the field.</summary>
    /// <param name="editor">The registered editor.</param>
    public EditorTagHelper(IRichTextEditor editor)
    {
        _editor = editor;
    }

    /// <summary>The model property bound to the textarea. The attribute is <c>asp-for</c>.</summary>
    [HtmlAttributeName(ForAttribute)]
    public ModelExpression For { get; set; } = default!;

    /// <summary>Editor height in pixels. The attribute is <c>height</c>. Overrides the registered editor's height.</summary>
    public int? Height { get; set; }

    /// <summary>Minimum height for this field, in pixels. Overrides the registered editor's minimum.</summary>
    [HtmlAttributeName("min-height")]
    public int? MinHeight { get; set; }

    /// <summary>Maximum height for this field, in pixels. Overrides the registered editor's maximum.</summary>
    [HtmlAttributeName("max-height")]
    public int? MaxHeight { get; set; }

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

    /// <summary>
    /// When false, this field keeps the editor's own file dialog even if a file manager is registered.
    /// Other editors on the same page are unchanged.
    /// </summary>
    [HtmlAttributeName("file-manager")]
    public bool FileManager { get; set; } = true;

    /// <summary>The current view. The startup scripts are written once for this request.</summary>
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    /// <inheritdoc/>
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

        if (MinHeight is int minHeight)
        {
            options["minHeight"] = minHeight;
        }

        if (MaxHeight is int maxHeight)
        {
            options["maxHeight"] = maxHeight;
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

        if (!FileManager)
        {
            options["fileManager"] = false;
        }

        return options;
    }
}