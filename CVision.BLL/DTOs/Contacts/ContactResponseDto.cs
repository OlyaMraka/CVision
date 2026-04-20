namespace CVision.BLL.DTOs.Contacts;

public class ContactResponseDto
{
    public int Id { get; set; }

    public int ContactUserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
