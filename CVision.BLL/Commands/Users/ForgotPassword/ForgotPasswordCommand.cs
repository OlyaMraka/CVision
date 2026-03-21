using CVision.BLL.DTOs.Users;
using FluentResults;
using MediatR;

namespace CVision.BLL.Commands.Users.ForgotPassword;

public record ForgotPasswordCommand(ForgotPasswordRequestDto RequestDto)
    : IRequest<Result>;
