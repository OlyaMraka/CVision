using CVision.BLL.Constans;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CVision.BLL.Commands.Users.ConfirmEmail;

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Result<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ConfirmEmailHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(request.RequestDto.UserId.ToString());
        if (user == null)
        {
            return UserConstants.UserNotFound;
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.RequestDto.Token);

        if (!result.Succeeded)
        {
            return UserConstants.EmailConfirmationError;
        }

        return true;
    }
}
