namespace CVision.Models.ViewModels.CVAnalysisViewModels;

public class CVGalleryPageViewModel
{
    public ICollection<CVGalleryViewModel> Items { get; set; }
        = new List<CVGalleryViewModel>();
}