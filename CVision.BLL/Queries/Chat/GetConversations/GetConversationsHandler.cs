using CVision.BLL.DTOs.Chat;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Queries.Chat.GetConversations;

public class GetConversationsHandler(
    IRepositoryWrapper repositoryWrapper) : IRequestHandler<GetConversationsQuery, Result<IEnumerable<ConversationSummaryDto>>>
{
    public async Task<Result<IEnumerable<ConversationSummaryDto>>> Handle(
        GetConversationsQuery request,
        CancellationToken cancellationToken)
    {
        var messages = await repositoryWrapper.ChatMessageRepository.GetAllAsync(
            new QueryOptions<ChatMessage>
            {
                Filter = x => !x.IsDeleted
                              && (x.SenderId == request.CurrentUserId || x.ReceiverId == request.CurrentUserId),
                Include = x => x.Include(m => m.Sender).Include(m => m.Receiver),
            });

        var messageList = messages.ToList();

        var summaries = messageList
            .GroupBy(m => m.SenderId == request.CurrentUserId ? m.ReceiverId : m.SenderId)
            .Select(group =>
            {
                var last = group.OrderByDescending(m => m.CreatedAt).First();
                var otherUser = last.SenderId == request.CurrentUserId ? last.Receiver : last.Sender;
                var unreadCount = group.Count(m => m.ReceiverId == request.CurrentUserId && !m.IsRead);

                return new ConversationSummaryDto
                {
                    OtherUserId = group.Key,
                    OtherUserName = otherUser?.UserName ?? string.Empty,
                    LastMessageContent = last.Content,
                    LastMessageAt = last.CreatedAt,
                    LastMessageSenderId = last.SenderId,
                    UnreadCount = unreadCount,
                };
            })
            .OrderByDescending(s => s.LastMessageAt)
            .ToList();

        return Result<IEnumerable<ConversationSummaryDto>>.Ok(summaries);
    }
}
