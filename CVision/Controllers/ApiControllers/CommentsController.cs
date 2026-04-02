using CVision.BLL.Commands.Comments.Create;
using CVision.BLL.DTOs.Comments;
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
}