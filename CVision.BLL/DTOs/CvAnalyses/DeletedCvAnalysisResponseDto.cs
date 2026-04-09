namespace CVision.BLL.DTOs.CvAnalyses;

public class DeletedCvAnalysisResponseDto
{
    public int Id { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public int Days { get; set; }
}