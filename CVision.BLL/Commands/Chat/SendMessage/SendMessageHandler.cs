using AutoMapper;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Chat;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Commands.Chat.SendMessage;

public class SendMessageHandler(
    IRepositoryWrapper repositoryWrapper,
    IMapper mapper,
    UserManager<ApplicationUser> userManager,
    IValidator<SendMessageCommand> validator) : IRequestHandler<SendMessageCommand, Result<ChatMessageDto>>
{
    public async Task<Result<ChatMessageDto>> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.First().ErrorMessage;
        }

        if (request.SenderId == request.RequestDto.ReceiverId)
        {
            return ChatConstants.CannotMessageSelf;
        }

        var receiver = await userManager.FindByIdAsync(request.RequestDto.ReceiverId.ToString());
        if (receiver is null)
        {
            return ChatConstants.ReceiverNotFound;
        }

        var message = new ChatMessage
        {
            SenderId = request.SenderId,
            ReceiverId = request.RequestDto.ReceiverId,
            Content = request.RequestDto.Content,
        };

        await repositoryWrapper.ChatMessageRepository.CreateAsync(message);

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return ChatConstants.SaveMessageError;
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
