using CVision.BLL.Constans;
using MediatR;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Commands.CvAnalyses.Recover;

public class RecoverCvAnalysisHandler(IRepositoryWrapper repositoryWrapper) : IRequestHandler<RecoverCvAnalysisCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RecoverCvAnalysisCommand request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<CVAnalysis>
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

        result.IsDeleted = false;
        result.DeletedAt = null;

        result.CV.IsDeleted = false;
        result.CV.DeletedAt = null;

        foreach (var recommendation in result.Recommendations)
        {
            recommendation.IsDeleted = false;
            recommendation.DeletedAt = null;
        }

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return CvAnalysisConstants.DbRecoverError;
        }

        return true;
    }
}
