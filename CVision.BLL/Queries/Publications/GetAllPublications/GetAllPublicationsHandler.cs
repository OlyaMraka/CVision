using AutoMapper;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Queries.Publications.GetAllPublications;

public class GetAllPublicationsHandler(
    IRepositoryWrapper repositoryWrapper,
    IMapper mapper) : IRequestHandler<GetAllPublicationsQuery, Result<IEnumerable<PublicationResponseShortDto>>>
{
    public async Task<Result<IEnumerable<PublicationResponseShortDto>>> Handle(
        GetAllPublicationsQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<Publication>
        {
            Include = p => p.Include(x => x.CV)
                .Include(x => x.User),
        };

        var publications = await repositoryWrapper.PublicationRepository.GetAllAsync(queryOptions);

        var response = mapper.Map<IEnumerable<PublicationResponseShortDto>>(publications);

        return Result<IEnumerable<PublicationResponseShortDto>>.Ok(response);
    }
}
