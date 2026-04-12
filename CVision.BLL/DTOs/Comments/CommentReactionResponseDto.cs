using CVision.DAL.Entities;

namespace CVision.BLL.DTOs.Comments;

public class CommentReactionResponseDto
{
    public int Likes { get; set; }

    public int Dislikes { get; set; }

    public ReactionType? CurrentUserReaction { get; set; }
}
