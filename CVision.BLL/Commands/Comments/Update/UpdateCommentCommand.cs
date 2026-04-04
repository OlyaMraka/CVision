using CVision.BLL.DTOs.Comments;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Comments.Update;

public record UpdateCommentCommand(UpdateCommentRequestDto RequestDto)
    : IRequest<Result<CommentResponseDto>>;
