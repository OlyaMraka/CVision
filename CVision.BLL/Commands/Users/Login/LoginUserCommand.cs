using CVision.BLL.DTOs.Users;
using CVision.DAL.Entities;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Users.Login;

public record LoginUserCommand(LoginUserRequestDto RequestDto)
    : IRequest<Result<ApplicationUser>>;
