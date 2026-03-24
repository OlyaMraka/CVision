namespace CVision.DAL.Entities;

public class CV
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public string PublicId { get; set; } = string.Empty;

    public string? ParsedText { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<CVAnalysis> Analyses { get; set; } = new List<CVAnalysis>();

    public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();
}
