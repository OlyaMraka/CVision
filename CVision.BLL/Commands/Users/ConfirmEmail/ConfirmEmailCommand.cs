using CVision.BLL.DTOs.Users;
using MediatR;
using FluentResults;

namespace CVision.BLL.Commands.Users.ConfirmEmail;

public record ConfirmEmailCommand(ConfirmEmailRequestDto RequestDto)
    : IRequest<Result>
{
}
