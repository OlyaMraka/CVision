using CVision.DAL.Data;
using CVision.DAL.Repositories.Interfaces.Base;

namespace CVision.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private ApplicationDbContext context;

    public RepositoryWrapper(ApplicationDbContext dbContext)
    {
        context = dbContext;
    }

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