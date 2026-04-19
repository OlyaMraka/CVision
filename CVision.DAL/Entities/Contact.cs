namespace CVision.DAL.Entities;

public class Contact
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public int ContactUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ApplicationUser Owner { get; set; } = null!;

    public virtual ApplicationUser ContactUser { get; set; } = null!;
}
