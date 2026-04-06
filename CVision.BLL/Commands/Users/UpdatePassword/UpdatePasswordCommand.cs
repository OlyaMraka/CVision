using CVision.BLL.DTOs.Users;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Users.UpdatePassword;

public record UpdatePasswordCommand(int UserId, UpdatePasswordRequestDto RequestDto)
    : IRequest<Result<bool>>;
