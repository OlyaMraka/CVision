using CVision.DAL.Data;
using CVision.DAL.Repositories.Interfaces.User;
using CVision.DAL.Repositories.Realizations.Base;
using Microsoft.AspNetCore.Identity;

namespace CVision.DAL.Repositories.Realizations.User;

public class UserTokenRepository : RepositoryBase<IdentityUserToken<int>>, IUserTokenRepository
{
    public UserTokenRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
