using CVision.BLL.DTOs.Users;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.Users.GetUserById;

public record GetUserByIdQuery(int UserId)
    : IRequest<Result<GetUserResponseDto>>;
