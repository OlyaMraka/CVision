using CVision.DAL.Data;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Comments;
using CVision.DAL.Repositories.Realizations.Base;

namespace CVision.DAL.Repositories.Realizations.Comments;

public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
{
    public CommentRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
