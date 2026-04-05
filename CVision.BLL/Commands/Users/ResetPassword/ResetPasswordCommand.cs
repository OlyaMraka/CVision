using CVision.BLL.DTOs.Users;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Users.ResetPassword;

public record ResetPasswordCommand(ResetPasswordRequestDto RequestDto)
    : IRequest<Result<bool>>;
