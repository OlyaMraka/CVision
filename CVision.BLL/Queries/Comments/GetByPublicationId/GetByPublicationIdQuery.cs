using CVision.BLL.DTOs.Comments;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.Comments.GetByPublicationId;

public record GetByPublicationIdQuery(int PublicationId)
    : IRequest<Result<IEnumerable<CommentResponseDto>>>;