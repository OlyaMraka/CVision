namespace CVision.BLL.DTOs.CvAnalyses;

public class CvAnalysisResultDto
{
    public string FeedBack { get; set; } = string.Empty;

    public int Score { get; set; }

    public ICollection<CvSectionAnalisysResultDto> SectionsResults { get; set; }
        = new List<CvSectionAnalisysResultDto>();
}