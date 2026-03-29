using System.ComponentModel.DataAnnotations;

namespace CVision.Models.ViewModels.CVForumViewModels;

public class CreateCVForumPostViewModel
{
    [Required(ErrorMessage = "Введіть заголовок")]
    [StringLength(100, MinimumLength = 3)]
    [Display(Name = "Заголовок")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введіть опис")]
    [StringLength(1000, MinimumLength = 10)]
    [Display(Name = "Опис")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Завантажте файл")]
    [Display(Name = "Файл")]
    public IFormFile File { get; set; } = null!;
}