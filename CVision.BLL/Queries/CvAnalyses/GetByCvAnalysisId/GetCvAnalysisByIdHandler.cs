using AutoMapper;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Queries.CvAnalyses.GetByCvAnalysisId;

public class GetCvAnalysisByIdHandler(
    IRepositoryWrapper repositoryWrapper,
    IMapper mapper) : IRequestHandler<GetCvAnalysisByIdQuery, Result<CvAnalysisInfoResponseDto>>
{
    public async Task<Result<CvAnalysisInfoResponseDto>> Handle(
        GetCvAnalysisByIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<CVAnalysis>
        {
            Filter = x => x.Id == request.Id,
            Include = cv
                => cv.Include(x => x.CV)
                    .Include(x => x.Recommendations),
        };

        var cvAnalysis = await repositoryWrapper.CvAnalysisRepository.GetFirstOrDefaultAsync(queryOptions);

        var result = mapper.Map<CvAnalysisInfoResponseDto>(cvAnalysis);

        return result;
    }
}
