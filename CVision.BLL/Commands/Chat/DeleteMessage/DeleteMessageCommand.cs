using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Chat.DeleteMessage;

public record DeleteMessageCommand(int SenderId, int MessageId) : IRequest<Result<bool>>;
