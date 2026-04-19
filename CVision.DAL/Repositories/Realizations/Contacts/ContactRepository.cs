using CVision.DAL.Data;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Contacts;
using CVision.DAL.Repositories.Realizations.Base;

namespace CVision.DAL.Repositories.Realizations.Contacts;

public class ContactRepository : RepositoryBase<Contact>, IContactRepository
{
    public ContactRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
