namespace CVision.DAL.Entities;

public class CommentReaction
{
    public int Id { get; set; }

    public int CommentId { get; set; }

    public int UserId { get; set; }

    public ReactionType ReactionType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Comment Comment { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;
}
