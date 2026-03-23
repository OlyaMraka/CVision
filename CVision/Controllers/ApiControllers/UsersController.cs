using CVision.BLL.Commands.Users.UpdateProfile;
using CVision.BLL.DTOs.Users;
using CVision.BLL.Queries.Users.GetUserById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CVision.Controllers;

public class UsersController : BaseApiController
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserResponseDto))]
    public async Task<IActionResult> GetUser([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetUserByIdQuery(id)));
    }

    [Authorize]
    [HttpPut("profile")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserResponseDto))]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto requestDto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return HandleResult(await Mediator.Send(new UpdateProfileCommand(userId, requestDto)));
    }
}