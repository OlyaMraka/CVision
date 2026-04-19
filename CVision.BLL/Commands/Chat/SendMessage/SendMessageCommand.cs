using CVision.BLL.DTOs.Chat;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Chat.SendMessage;

public record SendMessageCommand(int SenderId, SendMessageRequestDto RequestDto)
    : IRequest<Result<ChatMessageDto>>;
