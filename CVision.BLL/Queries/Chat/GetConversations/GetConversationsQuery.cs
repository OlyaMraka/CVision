using CVision.BLL.DTOs.Chat;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.Chat.GetConversations;

public record GetConversationsQuery(int CurrentUserId)
    : IRequest<Result<IEnumerable<ConversationSummaryDto>>>;
