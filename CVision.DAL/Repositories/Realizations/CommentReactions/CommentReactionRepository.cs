using CVision.DAL.Data;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.CommentReactions;
using CVision.DAL.Repositories.Realizations.Base;

namespace CVision.DAL.Repositories.Realizations.CommentReactions;

public class CommentReactionRepository : RepositoryBase<CommentReaction>, ICommentReactionRepository
{
    public CommentReactionRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
