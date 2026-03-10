namespace CVision.DAL.Entities;

public class Publication
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? CVId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual CV? CV { get; set; }
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
