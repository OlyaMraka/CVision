using Microsoft.AspNetCore.Identity;

namespace CVision.DAL.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual ICollection<CV> CVs { get; set; } = new List<CV>();
    public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
