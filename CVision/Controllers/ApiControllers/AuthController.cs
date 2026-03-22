using AutoMapper;
using CVision.BLL.Commands.Users.ConfirmEmail;
using CVision.BLL.Commands.Users.Login;
using CVision.BLL.Commands.Users.Register;
using CVision.BLL.DTOs.Users;
using CVision.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentResults;

namespace CVision.Controllers.ApiControllers;

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
}