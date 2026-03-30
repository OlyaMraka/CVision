using CVision.BLL.DTOs.Publications;
using MediatR;
using FluentResults;

namespace CVision.BLL.Queries.Publications.GetAllPublications;

public record GetAllPublicationsQuery()
    : IRequest<Result<IEnumerable<PublicationResponseShortDto>>>
{
}
