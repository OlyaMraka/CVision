using CVision.DAL.Data;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.User;
using CVision.DAL.Repositories.Realizations.Base;

namespace CVision.DAL.Repositories.Realizations.User;

public class ApplicationUserRepository : RepositoryBase<ApplicationUser>, IApplicationUserRepository
{
    public ApplicationUserRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
