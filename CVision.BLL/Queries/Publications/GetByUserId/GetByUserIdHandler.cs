using AutoMapper;
using FluentResults;
using MediatR;
using CVision.BLL.DTOs.Publications;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Queries.Publications.GetByUserId;

public class GetByUserIdHandler(
    IRepositoryWrapper repositoryWrapper,
    IMapper mapper) : IRequestHandler<GetByUserIdQuery, Result<IEnumerable<PublicationResponseShortDto>>>
{
    public async Task<Result<IEnumerable<PublicationResponseShortDto>>> Handle(
        GetByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<Publication>
        {
            Filter = x => x.UserId == request.UserId,
            Include = p => p.Include(x => x.CV)
                .Include(x => x.User),
        };

        var publications = await repositoryWrapper.PublicationRepository.GetAllAsync(queryOptions);

        var response = mapper.Map<IEnumerable<PublicationResponseShortDto>>(publications);

        return Result.Ok(response);
    }
}