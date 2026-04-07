namespace CVision.BLL.DTOs.Comments;

public class ParentCommentDto
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string? Content { get; set; }

    public bool IsDeleted { get; set; }
}