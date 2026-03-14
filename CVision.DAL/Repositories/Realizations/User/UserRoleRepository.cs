using CVision.DAL.Data;
using CVision.DAL.Repositories.Interfaces.User;
using CVision.DAL.Repositories.Realizations.Base;
using Microsoft.AspNetCore.Identity;

namespace CVision.DAL.Repositories.Realizations.User;

public class UserRoleRepository : RepositoryBase<IdentityUserRole<int>>, IUserRoleRepository
{
    public UserRoleRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
