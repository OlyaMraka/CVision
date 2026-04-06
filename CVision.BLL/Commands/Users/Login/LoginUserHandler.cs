using CVision.BLL.Constans;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
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
            return validationResult.Errors.First().ErrorMessage;
        }

        var user = await userManager.FindByEmailAsync(request.RequestDto.Email);

        if (user is null)
        {
            return UserConstants.UserLogInError;
        }

        var passwordValid = await userManager.CheckPasswordAsync(user, request.RequestDto.Password);

        if (!passwordValid)
        {
            return UserConstants.UserLogInError;
        }

        if (!user.EmailConfirmed)
        {
            return UserConstants.UserLogInError;
        }

        return user;
    }
}
