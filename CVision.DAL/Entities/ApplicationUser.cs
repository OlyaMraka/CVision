using Microsoft.AspNetCore.Identity;

namespace CVision.DAL.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<CV> CVs { get; set; } = new List<CV>();

    public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<CommentReaction> CommentReactions { get; set; } = new List<CommentReaction>();

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Contact> ContactOf { get; set; } = new List<Contact>();

    public virtual ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();

    public virtual ICollection<ChatMessage> ReceivedMessages { get; set; } = new List<ChatMessage>();
}
