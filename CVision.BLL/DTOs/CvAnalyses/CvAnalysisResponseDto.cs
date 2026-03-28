namespace CVision.BLL.DTOs.CvAnalyses;

public class CvAnalysisResponseDto
{
    public int Id { get; set; }

    public string FileUrl { get; set; } = string.Empty;

    public required CvAnalysisResultDto AnalysisResult { get; set; }
}