using CVision.BLL.DTOs.Users;
using MediatR;
using FluentResults;

namespace CVision.BLL.Queries.Users.GetUserById;

public record GetUserByIdQuery(int UserId)
    : IRequest<Result<GetUserResponseDto>>;