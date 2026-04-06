using CVision.BLL.DTOs.Users;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Users.ForgotPassword;

public record ForgotPasswordCommand(ForgotPasswordRequestDto RequestDto)
    : IRequest<Result<bool>>;
