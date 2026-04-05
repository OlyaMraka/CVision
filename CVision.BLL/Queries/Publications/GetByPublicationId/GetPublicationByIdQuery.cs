using CVision.BLL.DTOs.Publications;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.Publications.GetByPublicationId;

public record GetPublicationByIdQuery(int PublicationId)
    : IRequest<Result<PublicationResponseShortDto>>;
