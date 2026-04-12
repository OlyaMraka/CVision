using CVision.BLL.DTOs.Comments;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using MediatR;

namespace CVision.BLL.Commands.Comments.ToggleReaction;

public record ToggleReactionCommand(int CommentId, int UserId, ReactionType ReactionType)
    : IRequest<Result<CommentReactionResponseDto>>;
