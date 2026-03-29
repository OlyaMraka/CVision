using System.ComponentModel.DataAnnotations;

namespace CVision.Models.ViewModels.CVForumViewModels;

// ── Сторінка форуму ───────────────────────────────────────────
public class CVForumViewModel
{
    public ICollection<CVForumPostViewModel> AllPosts { get; set; }
        = new List<CVForumPostViewModel>();

    public ICollection<CVForumPostViewModel> MyPosts { get; set; }
        = new List<CVForumPostViewModel>();

    public string ActiveTab { get; set; } = "all";
}