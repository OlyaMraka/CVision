using CVision.BLL.DTOs.Publications;
using MediatR;
using FluentResults;

namespace CVision.BLL.Queries.Publications.GetByUserId;

public record GetByUserIdQuery(int UserId)
    : IRequest<Result<IEnumerable<PublicationResponseShortDto>>>
{
}