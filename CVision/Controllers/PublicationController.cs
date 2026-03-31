using AutoMapper;
using CVision.BLL.Commands.Publications.Create;
using CVision.BLL.Commands.Publications.Update;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.Queries.Publications.GetAllPublications;
using CVision.BLL.Queries.Publications.GetByPublicationId;
using CVision.Helpers.Constants;
using CVision.Models.ViewModels.CvForum;
using MediatR;
using CVision.BLL.Queries.Publications.GetByUserId;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

    [HttpGet]
    public async Task<IActionResult> PublicationPage(int id)
    {
        var result = await mediator.Send(new GetPublicationByIdQuery(id));
        if (result.IsFailed || result.ValueOrDefault is null)
        {
            return RedirectToAction(nameof(CvForum));
        }

        var model = mapper.Map<PublicationsViewModel>(result.Value);
        var currentUserId = GetCurrentUserId();
        model.IsOwner = currentUserId.HasValue && currentUserId.Value == model.UserId;

        return View("~/Views/CvForum/PublicationPage.cshtml", model);
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

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePublication(int id, string title, string description)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return RedirectToLogin();
        }

        var requestDto = new UpdatePublicationRequestDto
        {
            Title = title,
            Description = description,
        };

        var result = await mediator.Send(new UpdatePublicationCommand(id, userId.Value, requestDto));
        if (result.IsFailed)
        {
            TempData[PublicationConstants.EditErrorTempDataKey] = result.Errors.FirstOrDefault()?.Message
                ?? PublicationsConstants.PublicationUpdateError;
            return RedirectToAction(nameof(PublicationPage), new { id });
        }

        TempData[PublicationConstants.EditSuccessTempDataKey] = PublicationConstants.EditSuccessMessage;
        return RedirectToAction(nameof(PublicationPage), new { id });
    }
}