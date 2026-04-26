using CVision.BLL.DTOs.Chat;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Chat.EditMessage;

public record EditMessageCommand(int SenderId, EditMessageRequestDto RequestDto)
    : IRequest<Result<ChatMessageDto>>;
