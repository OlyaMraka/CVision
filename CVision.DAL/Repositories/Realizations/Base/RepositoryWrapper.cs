using CVision.DAL.Data;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.User;
using CVision.DAL.Repositories.Realizations.User;

namespace CVision.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private ApplicationDbContext context;
    private IApplicationUserRepository? applicationUserRepository;
    private IRoleRepository? roleRepository;
    private IUserRoleRepository? userRoleRepository;
    private IUserTokenRepository? userTokenRepository;

    public RepositoryWrapper(ApplicationDbContext dbContext)
    {
        context = dbContext;
    }

    public IApplicationUserRepository ApplicationUserRepository =>
        applicationUserRepository ??= new ApplicationUserRepository(context);

    public IRoleRepository RoleRepository => roleRepository ??= new RoleRepository(context);

    public IUserRoleRepository UserRoleRepository => userRoleRepository ??= new UserRoleRepository(context);

    public IUserTokenRepository UserTokenRepository => userTokenRepository ??= new UserTokenRepository(context);

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