using Microsoft.AspNetCore.Identity;
using CVision.DAL.Repositories.Interfaces.Base;

namespace CVision.DAL.Repositories.Interfaces.User;

public interface IUserTokenRepository : IRepositoryBase<IdentityUserToken<int>>
{
}
