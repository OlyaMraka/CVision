namespace CVision.BLL.DTOs.Comments;

public class CreateCommentRequestDto
{
    public int PublicationId { get; set; }

    public int UserId { get; set; }

    public int? ParentCommentId { get; set; }

    public string Content { get; set; } = string.Empty;
}