using AutoMapper;
using CVision.BLL.Commands.Publications.Create;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.Queries.Publications.GetAllPublications;
using CVision.Models.ViewModels.CvForum;
using MediatR;
using System.Security.Claims;
using CVision.BLL.Queries.Publications.GetByUserId;
using Microsoft.AspNetCore.Mvc;

namespace CVision.Controllers;

public class PublicationController(IMediator mediator, IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<IActionResult> CvForum()
    {
        var result = await mediator.Send(new GetAllPublicationsQuery());
        var parameters = new CvForumViewModel
        {
            Publications = mapper.Map<IEnumerable<PublicationViewModelShort>>(result.Value),
        };
        return View("~/Views/CvForum/CvForumMainPage.cshtml", parameters);
    }

    [HttpGet]
    public async Task<IActionResult> OwnPublications()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = int.TryParse(claim, out var id) ? id : 0;

        var result = await mediator.Send(new GetByUserIdQuery(userId));
        var parameters = new CvForumViewModel
        {
            Publications = mapper.Map<IEnumerable<PublicationViewModelShort>>(result.Value),
        };
        return View("~/Views/CvForum/CvForumMainPage.cshtml", parameters);
    }

    [HttpGet]
    public IActionResult CreateForm()
    {
        return View("~/Views/CvForum/CvForumCreateModal.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePublication(IFormFile file, string title, string description)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("file", "Будь ласка, завантажте файл");
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        using var stream = file!.OpenReadStream();

        var requestDto = new CreatePublicationRequestDto
        {
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            UserId = userId,
            Title = title,
            Description = description,
        };

        var result = await mediator.Send(new CreatePublicationCommand(requestDto));

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message ?? "Помилка аналізу CV";
            ModelState.AddModelError(string.Empty, errorMessage);
        }

        var publications = await mediator.Send(new GetAllPublicationsQuery());
        var parameters = new CvForumViewModel
        {
            Publications = mapper.Map<IEnumerable<PublicationViewModelShort>>(publications.Value),
        };
        return View("~/Views/CvForum/CvForumMainPage.cshtml", parameters);
    }
}