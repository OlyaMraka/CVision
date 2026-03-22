using CVision.BLL.DTOs.Users;
using CVision.BLL.Queries.Users.GetUserById;
using Microsoft.AspNetCore.Mvc;

namespace CVision.Controllers.ApiControllers;

public class UsersController : BaseApiController
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserResponseDto))]
    public async Task<IActionResult> GetUser([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetUserByIdQuery(id)));
    }
}