using CVision.DAL.Repositories.Interfaces.User;

namespace CVision.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    IApplicationUserRepository ApplicationUserRepository { get; }

    IRoleRepository RoleRepository { get; }

    IUserRoleRepository UserRoleRepository { get; }

    IUserTokenRepository UserTokenRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync();

    void ClearChangeTracker();
}