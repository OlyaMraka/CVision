using System.ComponentModel.DataAnnotations;

namespace CVision.Models.ViewModels.CVForumViewModels;

public class CVForumPostViewModel
{
    public int Id { get; set; }

    [Display(Name = "Заголовок")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Опис")]
    public string Description { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    [Display(Name = "Автор")]
    public string AuthorName { get; set; } = string.Empty;

    public string AuthorRole { get; set; } = string.Empty;

    [Display(Name = "Перегляди")]
    public int Views { get; set; }

    [Display(Name = "Коментарі")]
    public int Comments { get; set; }

    public bool IsOwner { get; set; }

    // 🔹 Для картки
    public string ShortDescription =>
        Description.Length > 140
            ? Description[..140].TrimEnd() + "..."
            : Description;
}