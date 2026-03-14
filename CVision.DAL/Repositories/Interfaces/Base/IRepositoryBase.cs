using CVision.DAL.Repositories.Options;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace CVision.DAL.Repositories.Interfaces.Base;

public interface IRepositoryBase<T>
    where T : class
{
    Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? queryOptions = null);

    Task<T?> GetFirstOrDefaultAsync(QueryOptions<T>? queryOptions = null);

    Task<T> CreateAsync(T entity);

    EntityEntry<T> Update(T entity);

    void Delete(T entity);
}
