using CVision.BLL.Constans;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;

namespace CVision.BLL.Commands.Chat.DeleteMessage;

public class DeleteMessageHandler(
    IRepositoryWrapper repositoryWrapper) : IRequestHandler<DeleteMessageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteMessageCommand request,
        CancellationToken cancellationToken)
    {
        var message = await repositoryWrapper.ChatMessageRepository.GetFirstOrDefaultAsync(
            new QueryOptions<ChatMessage>
            {
                Filter = x => x.Id == request.MessageId && !x.IsDeleted,
                AsNoTracking = false,
            });

        if (message is null)
        {
            return ChatConstants.MessageNotFound;
        }

        if (message.SenderId != request.SenderId)
        {
            return ChatConstants.NotMessageOwner;
        }

        message.IsDeleted = true;
        message.DeletedAt = DateTime.UtcNow;

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return ChatConstants.DeleteMessageError;
        }

        return true;
    }
}
