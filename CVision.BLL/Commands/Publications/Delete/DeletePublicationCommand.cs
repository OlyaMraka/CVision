using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Publications.Delete;

public record DeletePublicationCommand(int PublicationId, int UserId)
    : IRequest<Result<bool>>;
