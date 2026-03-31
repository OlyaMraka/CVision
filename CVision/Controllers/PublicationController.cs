using AutoMapper;
using CVision.BLL.Commands.Publications.Create;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.Queries.Publications.GetAllPublications;
using CVision.Models.ViewModels.CvForum;
using MediatR;
using CVision.BLL.Queries.Publications.GetByUserId;
using Microsoft.AspNetCore.Mvc;

namespace CVision.Controllers;

public class PublicationController(IMediator mediator, IMapper mapper) : BaseController
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
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return RedirectToLogin();
        }

        var result = await mediator.Send(new GetByUserIdQuery(userId.Value));
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
            ModelState.AddModelError("file", FileIsRequiredError);
        }

        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return RedirectToLogin();
        }

        using var stream = file!.OpenReadStream();

        var requestDto = new CreatePublicationRequestDto
        {
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            UserId = userId.Value,
            Title = title,
            Description = description,
        };

        var result = await mediator.Send(new CreatePublicationCommand(requestDto));

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message ?? CvAnalysisFailedError;
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