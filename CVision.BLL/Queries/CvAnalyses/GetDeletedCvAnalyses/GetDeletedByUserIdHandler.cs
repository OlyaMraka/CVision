using MediatR;
using AutoMapper;
using CVision.BLL.Constans;
using Microsoft.EntityFrameworkCore;
using CVision.BLL.Helpers;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;

namespace CVision.BLL.Queries.CvAnalyses.GetDeletedCvAnalyses;

public class GetDeletedByUserIdHandler(
    IRepositoryWrapper repositoryWrapper,
    IMapper mapper) : IRequestHandler<GetDeletedByUserIdQuery, Result<IEnumerable<DeletedCvAnalysisResponseDto>>>
{
    public async Task<Result<IEnumerable<DeletedCvAnalysisResponseDto>>> Handle(GetDeletedByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<CVAnalysis>
        {
            Filter = x => x.CV.UserId == request.UserId && x.IsDeleted,
            Include = x => x.Include(x => x.CV),
        };

        var cvAnalyses = await repositoryWrapper.CvAnalysisRepository.GetAllAsync(queryOptions);

        var filtered = cvAnalyses
            .Where(x => x.DeletedAt.HasValue && (DateTime.UtcNow - x.DeletedAt.Value).TotalDays < CvAnalysisConstants.DaysAlive)
            .ToList();

        var response = mapper.Map<IEnumerable<DeletedCvAnalysisResponseDto>>(filtered);

        return Result<IEnumerable<DeletedCvAnalysisResponseDto>>.Ok(response);
    }
}