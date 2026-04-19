using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Chat.MarkAsRead;

public record MarkConversationAsReadCommand(int CurrentUserId, int OtherUserId)
    : IRequest<Result<int>>;
