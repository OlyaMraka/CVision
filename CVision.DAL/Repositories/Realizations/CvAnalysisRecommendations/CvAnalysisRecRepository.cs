using CVision.DAL.Data;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.CvAnalysisRecommendations;
using CVision.DAL.Repositories.Realizations.Base;

namespace CVision.DAL.Repositories.Realizations.CvAnalysisRecommendations;

public class CvAnalysisRecRepository : RepositoryBase<CVAnalysisRecommendation>, ICvAnalysisRecRepository
{
    public CvAnalysisRecRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
