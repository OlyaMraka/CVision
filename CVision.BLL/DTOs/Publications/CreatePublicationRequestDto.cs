namespace CVision.BLL.DTOs.Publications;

public class CreatePublicationRequestDto
{
    public int UserId { get; set; }

    public required Stream FileStream { get; set; }

    public required string FileName { get; set; }

    public required string ContentType { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }
}