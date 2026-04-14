namespace CVision.Models.ViewModels.CvForum;

public class PublicationViewModelShort
{
    public int Id { get; set; }

    public string CreatorUserName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public DateTime PublishedAt { get; set; }
}
