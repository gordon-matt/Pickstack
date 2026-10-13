using System.ComponentModel.DataAnnotations;

namespace Pickstack.Demo.Models;

public sealed class ArticleForm
{
    [Required]
    public string? Title { get; set; }

    [Display(Name = "Cover image")]
    public string? CoverImage { get; set; }

    public string? Body { get; set; }
}
