using CVision.DAL.Data;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.CvAnalyses;
using CVision.DAL.Repositories.Realizations.Base;

namespace CVision.DAL.Repositories.Realizations.CvAnalyses;

public class CvAnalysisRepository : RepositoryBase<CVAnalysis>, ICvAnalysisRepository
{
    public CvAnalysisRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
