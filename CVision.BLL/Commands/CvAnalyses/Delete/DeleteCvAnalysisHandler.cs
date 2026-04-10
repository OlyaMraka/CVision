using CVision.BLL.Constans;
using MediatR;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Commands.CvAnalyses.Delete;

public class DeleteCvAnalysisHandler(IRepositoryWrapper repositoryWrapper) : IRequestHandler<DeleteCvAnalysisCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteCvAnalysisCommand request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<CVAnalysis>()
        {
            Filter = x => x.Id == request.CvAnalysisId,
            AsNoTracking = false,
            Include = x
                => x.Include(x => x.Recommendations)
                    .Include(x => x.CV),
        };

        var result = await repositoryWrapper.CvAnalysisRepository.GetFirstOrDefaultAsync(queryOptions);

        if (result == null)
        {
            return false;
        }

        result.IsDeleted = true;
        result.DeletedAt = DateTime.UtcNow;

        result.CV.IsDeleted = true;
        result.CV.DeletedAt = DateTime.UtcNow;

        foreach (var recommendation in result.Recommendations)
        {
            recommendation.IsDeleted = true;
            recommendation.DeletedAt = DateTime.UtcNow;
        }

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return CvAnalysisConstants.DbDeleteError;
        }

        return true;
    }
}