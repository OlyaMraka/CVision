using CVision.BLL.Commands.Publications.Delete;
using CVision.BLL.Commands.Publications.Update;
using CVision.BLL.DTOs.Publications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CVision.BLL.Queries.Publications.GetAllPublications;
using CVision.BLL.Queries.Publications.GetByPublicationId;
using CVision.BLL.Queries.Publications.GetByUserId;

namespace CVision.Controllers.ApiControllers;

public class PublicationsController : BaseApiController
{
    [Authorize]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePublication(
        [FromRoute] int id,
        [FromBody] UpdatePublicationRequestDto requestDto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return HandleResult(await Mediator.Send(new UpdatePublicationCommand(id, userId, requestDto)));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePublication([FromRoute] int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return HandleResult(await Mediator.Send(new DeletePublicationCommand(id, userId)));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublicationById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetPublicationByIdQuery(id)));
    }

    [HttpGet("user-publication/{userId:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUserId([FromRoute] int userId)
    {
        return HandleResult(await Mediator.Send(new GetByUserIdQuery(userId)));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPublications()
    {
        return HandleResult(await Mediator.Send(new GetAllPublicationsQuery()));
    }
}
