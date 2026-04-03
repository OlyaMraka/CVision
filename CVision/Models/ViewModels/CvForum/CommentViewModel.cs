namespace CVision.Models.ViewModels.CvForum;

public class CommentViewModel
{
    public int Id { get; set; }

    public string UserName { get; set; } = "Анонім";

    public string Content { get; set; } = string.Empty;

    public DateOnly CreatedOn { get; set; }

    public int Likes { get; set; }


    public ParentCommentViewModel? ParentComment { get; set; }

    public bool IsOwn { get; set; }
}

