using FluentResults;
using MediatR;
using CVision.BLL.DTOs.Users;

namespace CVision.BLL.Commands.Users.Register;

public record RegisterUserCommand(RegisterUserRequestDto RequestDto)
    : IRequest<Result<RegisterUserResponseDto>>;