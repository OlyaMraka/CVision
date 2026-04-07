using CVision.BLL.Constans;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;

namespace CVision.BLL.Commands.Comments.Delete;

public class DeleteCommentHandler(
    IRepositoryWrapper repositoryWrapper) : IRequestHandler<DeleteCommentCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = await repositoryWrapper.CommentRepository.GetFirstOrDefaultAsync(
            new QueryOptions<Comment>
            {
                Filter = x => x.Id == request.Id && !x.IsDeleted,
                AsNoTracking = false,
            });

        if (comment is null)
        {
            return CommentsConstants.CommentNotFound;
        }

        comment.IsDeleted = true;
        comment.DeletedAt = DateTime.UtcNow;

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return CommentsConstants.DeleteCommentError;
        }

        return true;
    }
}
