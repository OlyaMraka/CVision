using CVision.DAL.Data;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Publications;
using CVision.DAL.Repositories.Realizations.Base;

namespace CVision.DAL.Repositories.Realizations.Publications;

public class PublicationRepository : RepositoryBase<Publication>, IPublicationRepository
{
    public PublicationRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
