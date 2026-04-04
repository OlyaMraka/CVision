namespace CVision.BLL.DTOs.Comments;

public class CommentResponseDto
{
    public int Id { get; set; }

    public string? UserName { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public int Likes { get; set; }

    public ParentCommentDto? ParentComment { get; set; }
}