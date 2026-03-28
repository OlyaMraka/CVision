using CVision.BLL.DTOs.CvAnalyses;


namespace CVision.Models.ViewModels.CVAnalysisViewModels
{
    public class CVAnalysisViewModel
    {
        public int Id { get; set; }

        public string FileUrl { get; set; } = string.Empty;

        public required CVAnalysisResultViewModel AnalysisResult { get; set; }
    }
}
