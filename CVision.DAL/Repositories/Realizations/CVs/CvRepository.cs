using CVision.DAL.Data;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.CVs;
using CVision.DAL.Repositories.Realizations.Base;

namespace CVision.DAL.Repositories.Realizations.CVs;

public class CvRepository : RepositoryBase<CV>, ICvRepository
{
    public CvRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
