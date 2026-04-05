using CVision.BLL.DTOs.Publications;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.Publications.GetByUserId;

public record GetByUserIdQuery(int UserId)
    : IRequest<Result<IEnumerable<PublicationResponseShortDto>>>;
