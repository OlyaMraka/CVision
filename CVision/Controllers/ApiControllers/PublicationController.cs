using CVision.BLL.Commands.Publications.Create;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.Queries.Publications.GetAllPublications;
using CVision.BLL.Queries.Publications.GetByUserId;
using Microsoft.AspNetCore.Mvc;

namespace CVision.Controllers.ApiControllers;

public class PublicationController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUser()
    {
        return HandleResult(await Mediator.Send(new GetAllPublicationsQuery()));
    }

    [HttpGet("{userId:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUserId([FromRoute] int userId)
    {
        return HandleResult(await Mediator.Send(new GetByUserIdQuery(userId)));
    }

    // [HttpGet("{id:int}")]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IActionResult> GetUser([FromRoute] int id)
    // {
    //     return HandleResult(await Mediator.Send(new GetPublicationByIdQuery(id)));
    // }
}