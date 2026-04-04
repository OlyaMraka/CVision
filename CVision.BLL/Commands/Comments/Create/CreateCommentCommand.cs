using CVision.BLL.DTOs.Comments;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Comments.Create;

public record CreateCommentCommand(CreateCommentRequestDto RequestDto)
    : IRequest<Result<CommentResponseDto>>;