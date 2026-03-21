using CVision.DAL.Repositories.Interfaces.CvAnalyses;
using CVision.DAL.Repositories.Interfaces.CvAnalysisRecommendations;
using CVision.DAL.Repositories.Interfaces.CVs;

namespace CVision.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    public ICvRepository CvRepository { get; }

    public ICvAnalysisRepository CvAnalysisRepository { get; }

    public ICvAnalysisRecRepository CvAnalysisRecRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync();

    void ClearChangeTracker();
}