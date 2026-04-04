using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Comments.Delete;

public record DeleteCommentCommand(int Id) : IRequest<Result<bool>>;
