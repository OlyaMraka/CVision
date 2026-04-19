namespace CVision.BLL.DTOs.Chat;

public class ConversationSummaryDto
{
    public int OtherUserId { get; set; }

    public string OtherUserName { get; set; } = string.Empty;

    public string LastMessageContent { get; set; } = string.Empty;

    public DateTime LastMessageAt { get; set; }

    public int LastMessageSenderId { get; set; }

    public int UnreadCount { get; set; }
}
