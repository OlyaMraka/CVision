// Models/ViewModels/CVForumViewModels/CVForumViewModel.cs
// ══════════════════════════════════════════════════════════════
// ViewModels для сторінки CVForum.
// ══════════════════════════════════════════════════════════════

using CVision.BLL.DTOs.CVForum;
using System.ComponentModel.DataAnnotations;

namespace CVision.Models.ViewModels.CVForumViewModels
{
    // ── Головна сторінка форуму ────────────────────────────────
    public class CVForumViewModel
    {
        // Всі пости форуму
        public ICollection<CVForumPostViewModel> AllPosts { get; set; }
            = new List<CVForumPostViewModel>();

        // Тільки пости поточного юзера (вкладка "Мої резюме")
        public ICollection<CVForumPostViewModel> MyPosts { get; set; }
            = new List<CVForumPostViewModel>();

        // Активна вкладка: "all" або "my"
        public string ActiveTab { get; set; } = "all";

        public static CVForumViewModel FromDto(CVForumPageDto dto) =>
            new()
            {
                AllPosts = dto.AllPosts.Select(CVForumPostViewModel.FromDto).ToList(),
                MyPosts  = dto.MyPosts.Select(CVForumPostViewModel.FromDto).ToList(),
            };
    }


    // ── Одна картка поста ─────────────────────────────────────
    public class CVForumPostViewModel
    {
        public int    Id          { get; set; }
        public string Title       { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FileUrl     { get; set; } = string.Empty;
        public string AuthorName  { get; set; } = string.Empty;
        public string AuthorRole  { get; set; } = string.Empty;
        public int    Views       { get; set; }
        public int    Comments    { get; set; }
        public bool   IsOwner     { get; set; }

        // Скорочений опис — 3 рядки в картці
        public string ShortDescription => Description.Length > 140
            ? Description[..140].TrimEnd() + "..."
            : Description;

        public static CVForumPostViewModel FromDto(CVForumPostDto dto) =>
            new()
            {
                Id          = dto.Id,
                Title       = dto.Title,
                Description = dto.Description,
                FileUrl     = dto.FileUrl,
                AuthorName  = dto.AuthorName,
                AuthorRole  = dto.AuthorRole,
                Views       = dto.Views,
                Comments    = dto.Comments,
                IsOwner     = dto.IsOwner,
            };
    }


    // ── Форма створення поста (модалка "Додати резюме") ────────
    public class CreateCVForumPostViewModel
    {
        [Required(ErrorMessage = "Введіть заголовок")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Від 3 до 100 символів")]
        [Display(Name = "Заголовок")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть опис")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Від 10 до 1000 символів")]
        [Display(Name = "Опис")]
        public string Description { get; set; } = string.Empty;

        // Файл CV — обов'язковий
        [Required(ErrorMessage = "Завантажте файл резюме")]
        [Display(Name = "Файл резюме")]
        public IFormFile File { get; set; } = null!;
    }
}