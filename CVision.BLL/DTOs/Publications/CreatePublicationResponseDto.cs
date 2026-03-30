namespace CVision.BLL.DTOs.Publications;

public class CreatePublicationResponseDto
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public required string FileUrl { get; set; }

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
}