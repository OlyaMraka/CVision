using CVision.BLL.Constans;
using CVision.DAL.Entities;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CVision.BLL.Commands.Users.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ResetPasswordHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.RequestDto.Email);
        if (user == null)
        {
            return Result.Fail(UserConstants.PasswordResetError);
        }

        var result = await _userManager.ResetPasswordAsync(
            user, request.RequestDto.Token, request.RequestDto.NewPassword);

        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }
}
