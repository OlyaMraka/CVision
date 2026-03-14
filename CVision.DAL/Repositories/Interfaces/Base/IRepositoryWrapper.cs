namespace CVision.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    int SaveChanges();

    Task<int> SaveChangesAsync();

    void ClearChangeTracker();
}