namespace CVision.DAL.Entities;

public class CVAnalysis
{
    public int Id { get; set; }

    public int CVId { get; set; }

    public string? Feedback { get; set; }

    public int? Score { get; set; }

    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual CV CV { get; set; } = null!;

    public virtual ICollection<CVAnalysisRecommendation> Recommendations { get; set; } = new List<CVAnalysisRecommendation>();
}
