using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;

using CVision.BLL.DTOs.Publications;
using CVision.Models.ViewModels.CVForumViewModels;

using CVision.BLL.Commands.Publications.Create;


namespace CVision.Controllers;

public class CVForumController(IMediator mediator, IMapper mapper) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCVForumPostViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var dto = mapper.Map<CreatePublicationRequestDto>(model);
        dto.UserId = userId;

        var result = await mediator.Send(new CreatePublicationCommand(dto));

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Message);
        }

        var vm = mapper.Map<CVForumPostViewModel>(result.Value);

        vm.AuthorName = User.Identity!.Name!;
        vm.AuthorRole = "User";
        vm.IsOwner = true;

        return Ok(vm);
    }
}