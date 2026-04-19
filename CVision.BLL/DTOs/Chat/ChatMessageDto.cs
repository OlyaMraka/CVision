namespace CVision.BLL.DTOs.Chat;

public class ChatMessageDto
{
    public int Id { get; set; }

    public int SenderId { get; set; }

    public string SenderUserName { get; set; } = string.Empty;

    public int ReceiverId { get; set; }

    public string ReceiverUserName { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }
}
