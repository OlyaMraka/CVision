namespace CVision.DAL.Entities;

public class CVAnalysisRecommendation
{
    public int Id { get; set; }

    public int CVAnalysisId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual CVAnalysis CVAnalysis { get; set; } = null!;
}
