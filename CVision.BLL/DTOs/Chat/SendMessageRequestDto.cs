namespace CVision.BLL.DTOs.Chat;

public class SendMessageRequestDto
{
    public int ReceiverId { get; set; }

    public string Content { get; set; } = string.Empty;
}
