using CVision.BLL.Commands.Users.ConfirmEmail;
using CVision.BLL.Commands.Users.Register;
using CVision.BLL.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using FluentResults;

namespace CVision.Controllers;

public class AuthController : BaseApiController
{
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