using MediatR;
using AutoMapper;
using FluentResults;
using CVision.BLL.DTOs.Publications;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Queries.Publications.GetByPublicationId;

public class GetPublicationByIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    : IRequestHandler<GetPublicationByIdQuery, Result<PublicationResponseShortDto>>
{
    public async Task<Result<PublicationResponseShortDto>> Handle(
        GetPublicationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<Publication>
        {
            Filter = p => p.Id == request.PublicationId,
            Include = p => p.Include(x => x.CV)
                .Include(x => x.User),
        };

        var publications = await repositoryWrapper.PublicationRepository.GetFirstOrDefaultAsync(queryOptions);

        var response = mapper.Map<PublicationResponseShortDto>(publications);

        return Result.Ok(response);
    }
}