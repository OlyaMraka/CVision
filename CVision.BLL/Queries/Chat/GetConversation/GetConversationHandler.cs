using AutoMapper;
using CVision.BLL.DTOs.Chat;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Queries.Chat.GetConversation;

public class GetConversationHandler(
    IMapper mapper,
    IRepositoryWrapper repositoryWrapper) : IRequestHandler<GetConversationQuery, Result<IEnumerable<ChatMessageDto>>>
{
    public async Task<Result<IEnumerable<ChatMessageDto>>> Handle(
        GetConversationQuery request,
        CancellationToken cancellationToken)
    {
        var messages = await repositoryWrapper.ChatMessageRepository.GetAllAsync(
            new QueryOptions<ChatMessage>
            {
                Filter = x => (x.SenderId == request.CurrentUserId && x.ReceiverId == request.OtherUserId)
                              || (x.SenderId == request.OtherUserId && x.ReceiverId == request.CurrentUserId),
                Include = x => x.Include(m => m.Sender).Include(m => m.Receiver),
                Offset = request.Skip ?? 0,
                Limit = request.Take ?? 0,
            });

        var response = mapper.Map<IEnumerable<ChatMessageDto>>(messages)
            .OrderBy(x => x.CreatedAt)
            .ToList();

        return response;
    }
}
