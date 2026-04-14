using System.ComponentModel.DataAnnotations;

namespace CVision.Models.ViewModels.CVAnalysisViewModels
{
    public class CVGalleryViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Відгук")]
        public string FeedBack { get; set; } = string.Empty;

        [Display(Name = "Файл CV")]
        public string FileUrl { get; set; } = string.Empty;

        public string ShortFeedBack =>
            FeedBack.Length > 120
                ? FeedBack.Substring(0, 120) + "..."
                : FeedBack;
    }
}
