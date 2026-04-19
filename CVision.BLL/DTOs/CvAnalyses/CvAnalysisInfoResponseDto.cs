namespace CVision.BLL.DTOs.CvAnalyses;

public class CvAnalysisInfoResponseDto
{
    public int Id { get; set; }

    public int CVId { get; set; }

    public string FileUrl { get; set; } = string.Empty;

    public string FeedBack { get; set; } = string.Empty;

    public int Score { get; set; }

    public ICollection<CvSectionAnalisysResultDto> Recommendations { get; set; }
        = new List<CvSectionAnalisysResultDto>();
}