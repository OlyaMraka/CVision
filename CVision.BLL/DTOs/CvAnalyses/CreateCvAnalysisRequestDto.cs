namespace CVision.BLL.DTOs.CvAnalyses;

public class CreateCvAnalysisRequestDto
{
    public required Stream FileStream { get; set; }

    public required string FileName { get; set; }

    public required string ContentType { get; set; }

    public required int UserId { get; set; }
}