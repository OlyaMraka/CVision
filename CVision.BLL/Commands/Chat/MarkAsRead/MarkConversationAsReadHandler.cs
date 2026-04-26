using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;

namespace CVision.BLL.Commands.Chat.MarkAsRead;

public class MarkConversationAsReadHandler(
    IRepositoryWrapper repositoryWrapper) : IRequestHandler<MarkConversationAsReadCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        MarkConversationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var unread = await repositoryWrapper.ChatMessageRepository.GetAllAsync(
            new QueryOptions<ChatMessage>
            {
                Filter = x => !x.IsDeleted
                              && x.ReceiverId == request.CurrentUserId
                              && x.SenderId == request.OtherUserId
                              && !x.IsRead,
                AsNoTracking = false,
            });

        var now = DateTime.UtcNow;
        var count = 0;

        foreach (var message in unread)
        {
            message.IsRead = true;
            message.ReadAt = now;
            count++;
        }

        if (count == 0)
        {
            return 0;
        }

        await repositoryWrapper.SaveChangesAsync();

        return count;
    }
}
