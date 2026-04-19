using CVision.BLL.DTOs.Chat;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.Chat.GetConversation;

public record GetConversationQuery(int CurrentUserId, int OtherUserId, int? Skip = null, int? Take = null)
    : IRequest<Result<IEnumerable<ChatMessageDto>>>;
