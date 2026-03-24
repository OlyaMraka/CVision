using AutoMapper;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Users;
using CVision.DAL.Entities;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CVision.BLL.Commands.Users.UpdateProfile;

public class UpdateProfileHandler(
    UserManager<ApplicationUser> userManager,
    IMapper mapper) : IRequestHandler<UpdateProfileCommand, Result<GetUserResponseDto>>
{
    public async Task<Result<GetUserResponseDto>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return Result.Fail<GetUserResponseDto>(UserConstants.UserNotFound);
        }

        user.UserName = request.RequestDto.UserName;
        user.PhoneNumber = request.RequestDto.PhoneNumber;

        if (!string.Equals(user.Email, request.RequestDto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingUser = await userManager.FindByEmailAsync(request.RequestDto.Email);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                return Result.Fail<GetUserResponseDto>(UserConstants.EmailAlreadyInUse);
            }

            user.Email = request.RequestDto.Email;
            user.NormalizedEmail = userManager.NormalizeEmail(request.RequestDto.Email);
            user.EmailConfirmed = false;
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result.Fail<GetUserResponseDto>(errors);
        }

        var response = mapper.Map<GetUserResponseDto>(user);
        return Result.Ok(response);
    }
}
