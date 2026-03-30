namespace CVision.Models.ViewModels.CvForum;

public class CvForumViewModel
{
    public IEnumerable<PublicationViewModelShort> Publications { get; set; } = new List<PublicationViewModelShort>();
}