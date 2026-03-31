using CVision.BLL.Commands.Publications.Create;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.Queries.Publications.GetAllPublications;
using CVision.BLL.Queries.Publications.GetByUserId;
using Microsoft.AspNetCore.Mvc;

namespace CVision.Controllers.ApiControllers;

public class PublicationController : BaseApiController
{
    [HttpPost("create")]
    public async Task<IActionResult> CreatePost(
        [FromForm] IFormFile file,
        [FromForm] int userId,
        [FromForm] string title,
        [FromForm] string description)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Файл не вибрано.");
        }

        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var requestDto = new CreatePublicationRequestDto
        {
            FileStream = memoryStream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            UserId = userId,
            Title = title,
            Description = description,
        };

        using (memoryStream)
        {
            return HandleResult(await Mediator.Send(new CreatePublicationCommand(requestDto)));
        }
    }

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