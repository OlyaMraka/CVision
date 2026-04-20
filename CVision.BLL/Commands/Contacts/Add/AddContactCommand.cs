using CVision.BLL.DTOs.Contacts;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Contacts.Add;

public record AddContactCommand(int OwnerId, int ContactUserId)
    : IRequest<Result<ContactResponseDto>>;
