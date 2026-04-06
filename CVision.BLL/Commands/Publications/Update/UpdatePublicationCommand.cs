using CVision.BLL.DTOs.Publications;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Publications.Update;

public record UpdatePublicationCommand(int PublicationId, int UserId, UpdatePublicationRequestDto RequestDto)
    : IRequest<Result<bool>>;
