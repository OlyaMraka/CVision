using CVision.BLL.Constans;
using CVision.DAL.Entities;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CVision.BLL.Commands.Users.Login;

public class LoginUserHandler(
    UserManager<ApplicationUser> userManager,
    IValidator<LoginUserCommand> validator)
    : IRequestHandler<LoginUserCommand, Result<ApplicationUser>>
{
    public async Task<Result<ApplicationUser>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Fail<ApplicationUser>(validationResult.Errors.First().ErrorMessage);
        }

        var user = await userManager.FindByEmailAsync(request.RequestDto.Email);

        if (user is null)
        {
            return Result.Fail<ApplicationUser>(UserConstants.UserLogInError);
        }

        var passwordValid = await userManager.CheckPasswordAsync(user, request.RequestDto.Password);

        if (!passwordValid)
        {
            return Result.Fail<ApplicationUser>(UserConstants.UserLogInError);
        }

        return Result.Ok(user);
    }
}
