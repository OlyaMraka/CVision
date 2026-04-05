using CVision.BLL.DTOs.Users;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Users.ConfirmEmail;

public record ConfirmEmailCommand(ConfirmEmailRequestDto RequestDto)
    : IRequest<Result<bool>>;
