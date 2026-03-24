using AutoMapper;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.DAL.Entities;
using FluentResults;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Queries.CvAnalyses.GetAllCvAnalyses;

public class GetAllByUserIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper) : IRequestHandler<GetAllByUserIdQuery, Result<IEnumerable<CvAnalysisResponseShortDto>>>
{
    public async Task<Result<IEnumerable<CvAnalysisResponseShortDto>>> Handle(
        GetAllByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<CVAnalysis>
        {
            Filter = cv => cv.CV.UserId == request.UserId,
            Include = cv => cv.Include(x => x.CV),
        };

        var cvAnalyses = await repositoryWrapper.CvAnalysisRepository.GetAllAsync(queryOptions);

        var result = mapper.Map<IEnumerable<CvAnalysisResponseShortDto>>(cvAnalyses);

        return Result.Ok(result);
    }
}