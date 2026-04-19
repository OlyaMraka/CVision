using System.Security.Claims;
using CVision.BLL.Commands.Contacts.Add;
using CVision.BLL.Commands.Contacts.Remove;
using CVision.BLL.DTOs.Contacts;
using CVision.BLL.Queries.Contacts.GetAll;
using CVision.BLL.Queries.Contacts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVision.Controllers.ApiControllers;

[Authorize]
public class ContactsController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ContactResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyContacts()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return HandleResult(await Mediator.Send(new GetContactsQuery(userId)));
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserSearchResultDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchUsers([FromQuery] string query = "", [FromQuery] int limit = 20)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return HandleResult(await Mediator.Send(new SearchUsersQuery(userId, query, limit)));
    }

    [HttpPost("{contactUserId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContactResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddContact([FromRoute] int contactUserId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return HandleResult(await Mediator.Send(new AddContactCommand(userId, contactUserId)));
    }

    [HttpDelete("{contactUserId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveContact([FromRoute] int contactUserId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return HandleResult(await Mediator.Send(new RemoveContactCommand(userId, contactUserId)));
    }
}
