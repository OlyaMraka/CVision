using System.ComponentModel.DataAnnotations;



namespace CVision.Models.ViewModels.CVAnalysisViewModels
{
    public class CVAnalysisViewModel
    {
        [Display(Name = "Загальний бал")]
        public int Score { get; set; }

        [Display(Name = "Загальний відгук")]
        public string FeedBack { get; set; } = string.Empty;

        public ICollection<CVSectionResultViewModel> SectionResults { get; set; }
            = new List<CVSectionResultViewModel>();
    }
}
