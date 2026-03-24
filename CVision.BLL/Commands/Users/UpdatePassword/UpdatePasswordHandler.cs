using CVision.BLL.Constans;
using CVision.DAL.Entities;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CVision.BLL.Commands.Users.UpdatePassword;

public class UpdatePasswordHandler : IRequestHandler<UpdatePasswordCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdatePasswordHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return Result.Fail(UserConstants.UserNotFound);
        }

        var result = await _userManager.ChangePasswordAsync(
            user, request.RequestDto.CurrentPassword, request.RequestDto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            if (errors.Any(e => e.Contains("incorrect", StringComparison.OrdinalIgnoreCase)))
            {
                return Result.Fail(UserConstants.IncorrectCurrentPassword);
            }

            return Result.Fail(errors);
        }

        return Result.Ok();
    }
}
