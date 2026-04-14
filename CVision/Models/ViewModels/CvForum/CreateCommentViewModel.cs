namespace CVision.Models.ViewModels.CvForum;

public class CreateCommentViewModel
{
    public int PublicationId { get; set; }

    public string Content { get; set; } = string.Empty;

    public int? ParentCommentId { get; set; }
}
