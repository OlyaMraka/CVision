using AutoMapper;
using CVision.BLL.Commands.Users.ConfirmEmail;
using CVision.BLL.Commands.Users.ForgotPassword;
using CVision.BLL.Commands.Users.Login;
using CVision.BLL.Commands.Users.Register;
using CVision.BLL.Commands.Users.ResetPassword;
using CVision.BLL.Commands.Users.UpdatePassword;
using CVision.BLL.DTOs.Users;
using CVision.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentResults;
using System.Security.Claims;

namespace CVision.Controllers;

public class AuthController(
    SignInManager<ApplicationUser> signInManager,
    IMapper mapper) : BaseApiController
{
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogoutAsync()
    {
        await signInManager.SignOutAsync();
        return Ok();
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginUserResponseDto))]
    public async Task<IActionResult> LoginAsync(LoginUserRequestDto requestDto)
    {
        var result = await Mediator.Send(new LoginUserCommand(requestDto));

        if (result.IsFailed)
        {
            return HandleResult(result);
        }

        await signInManager.SignInAsync(result.Value, isPersistent: false);

        var responseDto = mapper.Map<LoginUserResponseDto>(result.Value);
        return Ok(responseDto);
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterUserResponseDto))]
    public async Task<IActionResult> RegisterAsync(RegisterUserRequestDto requestDto)
    {
        return HandleResult(await Mediator.Send(new RegisterUserCommand(requestDto)));
    }

    [HttpPost("confirm-email")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ConfirmEmailAsync(ConfirmEmailRequestDto requestDto)
    {
        return HandleResult(await Mediator.Send(new ConfirmEmailCommand(requestDto)));
    }

    [Authorize]
    [HttpPost("update-password")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePasswordAsync(UpdatePasswordRequestDto requestDto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return HandleResult(await Mediator.Send(new UpdatePasswordCommand(userId, requestDto)));
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequestDto requestDto)
    {
        await Mediator.Send(new ForgotPasswordCommand(requestDto));
        return Ok(new { message = "If the email exists, a password reset link has been sent." });
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequestDto requestDto)
    {
        return HandleResult(await Mediator.Send(new ResetPasswordCommand(requestDto)));
    }
}