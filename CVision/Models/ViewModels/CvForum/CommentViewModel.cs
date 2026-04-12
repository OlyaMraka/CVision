namespace CVision.Models.ViewModels.CvForum;

public class CommentViewModel
{
    public int Id { get; set; }

    public string UserName { get; set; } = "Анонім";

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int Likes { get; set; }

    public int Dislikes { get; set; }

    public int? CurrentUserReaction { get; set; } // 0 = Like, 1 = Dislike, null = no reaction

    public bool IsDeleted { get; set; }

    public ParentCommentViewModel? ParentComment { get; set; }

    public bool IsOwn { get; set; }
}


