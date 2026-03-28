namespace CVision.Models.ViewModels.CVAnalysisViewModels;

public class CVAnalysisResultViewModel
{
    public string FeedBack { get; set; } = string.Empty;

    public int Score { get; set; }

    public ICollection<CVSectionResultViewModel> SectionsResults { get; set; }
        = new List<CVSectionResultViewModel>();
}