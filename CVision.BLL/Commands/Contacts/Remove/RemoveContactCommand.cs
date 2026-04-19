using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Contacts.Remove;

public record RemoveContactCommand(int OwnerId, int ContactUserId)
    : IRequest<Result<bool>>;
