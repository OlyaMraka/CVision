namespace CVision.DAL.Entities;

public class Comment
{
    public int Id { get; set; }

    public int PublicationId { get; set; }

    public int UserId { get; set; }

    public int? ParentCommentId { get; set; }

    public string Content { get; set; } = string.Empty;

    public int Likes { get; set; } = 0;

    public int Dislikes { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    public virtual Publication Publication { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;

    public virtual Comment? ParentComment { get; set; }

    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();

    public virtual ICollection<CommentReaction> Reactions { get; set; } = new List<CommentReaction>();
}
