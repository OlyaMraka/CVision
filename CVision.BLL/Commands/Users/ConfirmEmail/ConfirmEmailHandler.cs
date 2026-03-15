using MediatR;
using FluentResults;
using CVision.DAL.Entities;
using CVision.BLL.Constans;
using Microsoft.AspNetCore.Identity;

namespace CVision.BLL.Commands.Users.ConfirmEmail;

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ConfirmEmailHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(request.RequestDto.UserId.ToString());
        if (user == null)
        {
            return Result.Fail(UserConstants.UserNotFound);
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.RequestDto.Token);

        if (!result.Succeeded)
        {
            return Result.Fail(UserConstants.EmailConfirmationError);
        }

        return Result.Ok();
    }
}