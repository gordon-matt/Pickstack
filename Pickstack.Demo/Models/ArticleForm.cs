using System.ComponentModel.DataAnnotations;

namespace Pickstack.Demo.Models;

public sealed class ArticleForm
{
    [Required]
    public string? Title { get; set; }

    [Display(Name = "Any")]
    public string? AnyFile { get; set; }

    [Display(Name = "File")]
    public string? Document { get; set; }

    [Display(Name = "Image")]
    public string? CoverImage { get; set; }

    [Display(Name = "Media")]
    public string? MediaFile { get; set; }

    [Display(Name = "Folder")]
    public string? Folder { get; set; }

    [Display(Name = "Rich text")]
    public string? Body { get; set; }
}