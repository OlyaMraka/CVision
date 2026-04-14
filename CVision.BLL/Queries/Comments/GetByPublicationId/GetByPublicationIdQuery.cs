using CVision.BLL.DTOs.Comments;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.Comments.GetByPublicationId;

public record GetByPublicationIdQuery(int PublicationId, int? CurrentUserId = null)
    : IRequest<Result<IEnumerable<CommentResponseDto>>>;