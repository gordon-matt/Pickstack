using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pickstack.FileManager;

/// <summary>
/// A <c>pickstack-file</c> field. It renders a text input, a Browse button, and, for an image, a preview.
/// The button calls <c>Pickstack.files.open</c>.
/// </summary>
[HtmlTargetElement("pickstack-file", Attributes = ForAttribute, TagStructure = TagStructure.WithoutEndTag)]
public sealed class FilePickerTagHelper : TagHelper
{
    private const string ForAttribute = "asp-for";

    private readonly IFileManager _fileManager;

    /// <summary>Uses the registered file manager for the startup scripts written into the field.</summary>
    /// <param name="fileManager">The registered file manager.</param>
    public FilePickerTagHelper(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    /// <summary>The model property bound to the text input. The attribute is <c>asp-for</c>.</summary>
    [HtmlAttributeName(ForAttribute)]
    public ModelExpression For { get; set; } = default!;

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
        if (!string.IsNullOrWhiteSpace(FileRoot))
        {
            output.Attributes.SetAttribute("data-file-root", FileRoot);
        }
        output.Content.SetHtmlContent(builder.ToString());
    }

    private static string Encode(string value) => System.Net.WebUtility.HtmlEncode(value);
}