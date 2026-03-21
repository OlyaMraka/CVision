using CVision.DAL.Data;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.CvAnalyses;
using CVision.DAL.Repositories.Interfaces.CvAnalysisRecommendations;
using CVision.DAL.Repositories.Interfaces.CVs;
using CVision.DAL.Repositories.Realizations.CvAnalyses;
using CVision.DAL.Repositories.Realizations.CvAnalysisRecommendations;
using CVision.DAL.Repositories.Realizations.CVs;

namespace CVision.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private ApplicationDbContext context;

    private ICvRepository? _cvRepository;

    private ICvAnalysisRepository? _cvAnalysisRepository;

    private ICvAnalysisRecRepository? _cvAnalysisRecRepository;

    public RepositoryWrapper(ApplicationDbContext dbContext)
    {
        context = dbContext;
    }

    public ICvRepository CvRepository
        => _cvRepository ??= new CvRepository(context);

    public ICvAnalysisRepository CvAnalysisRepository
        => _cvAnalysisRepository ??= new CvAnalysisRepository(context);

    public ICvAnalysisRecRepository CvAnalysisRecRepository
        => _cvAnalysisRecRepository ??= new CvAnalysisRecRepository(context);

    public int SaveChanges()
    {
        return context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }

    public void ClearChangeTracker()
    {
        context.ChangeTracker.Clear();
    }
}