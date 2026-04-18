namespace CVision.Models.ViewModels.CVAnalysisViewModels;

public class CVAnalysisInfoViewModel
{
    public int Id { get; set; }

    public string FileUrl { get; set; } = string.Empty;

    public string FeedBack { get; set; } = string.Empty;

    public int Score { get; set; }

    public ICollection<CVSectionResultViewModel> Recommendations { get; set; }
        = new List<CVSectionResultViewModel>();
}