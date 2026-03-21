using CVision.BLL.DTOs.Users;
using FluentResults;
using MediatR;

namespace CVision.BLL.Commands.Users.ResetPassword;

public record ResetPasswordCommand(ResetPasswordRequestDto RequestDto)
    : IRequest<Result>;
