using CVision.BLL.DTOs.Publications;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.Publications.GetAllPublications;

public record GetAllPublicationsQuery()
    : IRequest<Result<IEnumerable<PublicationResponseShortDto>>>;
