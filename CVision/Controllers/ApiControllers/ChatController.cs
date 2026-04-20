using System.Security.Claims;
using CVision.BLL.Commands.Chat.MarkAsRead;
using CVision.BLL.Commands.Chat.SendMessage;
using CVision.BLL.DTOs.Chat;
using CVision.BLL.Queries.Chat.GetConversation;
using CVision.BLL.Queries.Chat.GetConversations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVision.Controllers.ApiControllers;

[Authorize]
public class ChatController : BaseApiController
{
    [HttpGet("conversations")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ConversationSummaryDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetConversations()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return HandleResult(await Mediator.Send(new GetConversationsQuery(userId)));
    }

    [HttpGet("conversation/{otherUserId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ChatMessageDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetConversation(
        [FromRoute] int otherUserId,
        [FromQuery] int? skip = null,
        [FromQuery] int? take = null)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return HandleResult(await Mediator.Send(new GetConversationQuery(userId, otherUserId, skip, take)));
    }

    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChatMessageDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequestDto requestDto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return HandleResult(await Mediator.Send(new SendMessageCommand(userId, requestDto)));
    }

    [HttpPost("conversation/{otherUserId:int}/mark-read")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkConversationAsRead([FromRoute] int otherUserId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return HandleResult(await Mediator.Send(new MarkConversationAsReadCommand(userId, otherUserId)));
    }
}
