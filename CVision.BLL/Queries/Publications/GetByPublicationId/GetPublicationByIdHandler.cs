using MediatR;
using AutoMapper;
using FluentResults;
using CVision.BLL.DTOs.Publications;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using Microsoft.EntityFrameworkCore;
using CVision.BLL.Constans;

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
            Filter = x => x.Id == request.PublicationId,
            Include = p => p.Include(x => x.CV)
                .Include(x => x.User),
        };

        var publication = await repositoryWrapper.PublicationRepository.GetFirstOrDefaultAsync(queryOptions);
        if (publication is null)
        {
            return Result.Fail(PublicationsConstants.PublicationNotFound);
        }

        var response = mapper.Map<PublicationResponseShortDto>(publication);

        return Result.Ok(response);
    }
}