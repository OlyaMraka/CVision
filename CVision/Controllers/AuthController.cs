using CVision.BLL.Commands.Users.Register;
using CVision.BLL.DTOs.Users;
using Microsoft.AspNetCore.Mvc;

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
}