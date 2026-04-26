using AutoMapper;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Chat;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Commands.Chat.EditMessage;

public class EditMessageHandler(
    IRepositoryWrapper repositoryWrapper,
    IMapper mapper,
    IValidator<EditMessageCommand> validator) : IRequestHandler<EditMessageCommand, Result<ChatMessageDto>>
{
    public async Task<Result<ChatMessageDto>> Handle(
        EditMessageCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.First().ErrorMessage;
        }

        var message = await repositoryWrapper.ChatMessageRepository.GetFirstOrDefaultAsync(
            new QueryOptions<ChatMessage>
            {
                Filter = x => x.Id == request.RequestDto.Id && !x.IsDeleted,
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

        message.Content = request.RequestDto.Content;
        message.IsEdited = true;
        message.EditedAt = DateTime.UtcNow;

        repositoryWrapper.ChatMessageRepository.Update(message);

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return ChatConstants.UpdateMessageError;
        }

        var saved = await repositoryWrapper.ChatMessageRepository.GetFirstOrDefaultAsync(
            new QueryOptions<ChatMessage>
            {
                Filter = x => x.Id == message.Id,
                Include = x => x.Include(m => m.Sender).Include(m => m.Receiver),
            });

        return mapper.Map<ChatMessageDto>(saved);
    }
}
