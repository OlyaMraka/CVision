using CVision.BLL.Commands.Comments.Create;
using CVision.BLL.Commands.Comments.Delete;
using CVision.BLL.Commands.Comments.Update;
using CVision.BLL.DTOs.Comments;
using CVision.BLL.Queries.Comments.GetByPublicationId;
using Microsoft.AspNetCore.Mvc;

namespace CVision.Controllers.ApiControllers;

public class CommentsController : BaseApiController
{
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequestDto requestDto)
    {
        return Ok(await Mediator.Send(new CreateCommentCommand(requestDto)));
    }

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentRequestDto requestDto)
    {
        return Ok(await Mediator.Send(new UpdateCommentCommand(requestDto)));
    }

    [HttpDelete("delete/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment([FromRoute] int id)
    {
        return Ok(await Mediator.Send(new DeleteCommentCommand(id)));
    }

    [HttpGet("publication-comments/{publicationId:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCommentsByPublicationId([FromRoute] int publicationId)
    {
        return Ok(await Mediator.Send(new GetByPublicationIdQuery(publicationId)));
    }
}