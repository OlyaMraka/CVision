using CVision.BLL.Constans;
using CVision.BLL.DTOs.Comments;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;

namespace CVision.BLL.Commands.Comments.ToggleReaction;

public class ToggleReactionHandler(
    IRepositoryWrapper repositoryWrapper) : IRequestHandler<ToggleReactionCommand, Result<CommentReactionResponseDto>>
{
    public async Task<Result<CommentReactionResponseDto>> Handle(
        ToggleReactionCommand request,
        CancellationToken cancellationToken)
    {
        var comment = await repositoryWrapper.CommentRepository.GetFirstOrDefaultAsync(
            new QueryOptions<Comment>
            {
                Filter = x => x.Id == request.CommentId && !x.IsDeleted,
                AsNoTracking = false,
            });

        if (comment is null)
        {
            return CommentsConstants.CommentNotFound;
        }

        var existingReaction = await repositoryWrapper.CommentReactionRepository.GetFirstOrDefaultAsync(
            new QueryOptions<CommentReaction>
            {
                Filter = x => x.CommentId == request.CommentId && x.UserId == request.UserId,
                AsNoTracking = false,
            });

        ReactionType? currentUserReaction;

        if (existingReaction is null)
        {
            await repositoryWrapper.CommentReactionRepository.CreateAsync(new CommentReaction
            {
                CommentId = request.CommentId,
                UserId = request.UserId,
                ReactionType = request.ReactionType,
            });

            if (request.ReactionType == ReactionType.Like)
            {
                comment.Likes++;
            }
            else
            {
                comment.Dislikes++;
            }

            currentUserReaction = request.ReactionType;
        }
        else if (existingReaction.ReactionType == request.ReactionType)
        {
            if (existingReaction.ReactionType == ReactionType.Like)
            {
                comment.Likes--;
            }
            else
            {
                comment.Dislikes--;
            }

            repositoryWrapper.CommentReactionRepository.Delete(existingReaction);
            currentUserReaction = null;
        }
        else
        {
            if (existingReaction.ReactionType == ReactionType.Like)
            {
                comment.Likes--;
                comment.Dislikes++;
            }
            else
            {
                comment.Dislikes--;
                comment.Likes++;
            }

            existingReaction.ReactionType = request.ReactionType;
            currentUserReaction = request.ReactionType;
        }

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return CommentsConstants.ReactionError;
        }

        return new CommentReactionResponseDto
        {
            Likes = comment.Likes,
            Dislikes = comment.Dislikes,
            CurrentUserReaction = currentUserReaction,
        };
    }
}
