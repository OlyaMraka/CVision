using CVision.DAL.Data;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.CvLookups;
using CVision.DAL.Repositories.Realizations.Base;

namespace CVision.DAL.Repositories.Realizations.CvLookups;

public class CvLookupRepository : RepositoryBase<CvLookup>, ICvLookupRepository
{
    public CvLookupRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
