using AutoMapper;
using CVision.BLL.Commands.Publications.Create;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.Queries.Publications.GetAllPublications;
using CVision.Models.ViewModels.CvForum;
using MediatR;
using System.Security.Claims;
using CVision.BLL.Commands.Publications.Delete;
using CVision.BLL.Commands.Publications.Update;
using CVision.BLL.Queries.Publications.GetByPublicationId;
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
    public async Task<IActionResult> GetPublication(int publicationId)
    {
        var userId = GetUserId();

        var result = await mediator.Send(new GetPublicationByIdQuery(publicationId));
        var parameter = mapper.Map<PublicationsViewModel>(result.Value);
        parameter.IsOwn = userId == parameter.UserId;
        return View("~/Views/CvForum/PublicationPage.cshtml", parameter);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePublication(int publicationId)
    {
        var userId = GetUserId();

        var result = await mediator.Send(new DeletePublicationCommand(publicationId, userId));

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message ?? "Не вдалося видалити публікацію";

            var backUrl = Url.Action("GetPublication", "Publication", new { publicationId });

            return RedirectToAction("ShowError", "Publication", new
            {
                message = errorMessage,
                returnUrl = backUrl,
            });
        }

        return RedirectToAction("CvForum");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmDelete(int publicationId)
    {
        var result = await mediator.Send(new GetPublicationByIdQuery(publicationId));
        var parameter = mapper.Map<ConfirmationViewModal>(result.Value);
        return View("~/Views/CvForum/ConfirmationPopup.cshtml", parameter);
    }

    [HttpGet]
    public async Task<IActionResult> OwnPublications()
    {
        var userId = GetUserId();

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

    [HttpGet]
    public async Task<IActionResult> EditPublication(int publicationId)
    {
        var result = await mediator.Send(new GetPublicationByIdQuery(publicationId));

        var model = mapper.Map<PublicationsViewModel>(result.Value);

        return View("~/Views/CvForum/EditPublication.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPublication(PublicationsViewModel model)
    {
        var userId = GetUserId();

        var requestDto = new UpdatePublicationRequestDto
        {
            Title = model.Title,
            Description = model.Description,
        };

        var command = new UpdatePublicationCommand(model.Id, userId, requestDto);
        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message ?? "Невідома помилка при оновленні";
            var backUrl = Url.Action("GetPublication", "Publication", new { publicationId = model.Id });

            return RedirectToAction("ShowError", "Publication", new
            {
                message = errorMessage,
                returnUrl = backUrl,
            });
        }

        return RedirectToAction("GetPublication", new { publicationId = model.Id });
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
            var errorMessage = result.Errors.FirstOrDefault()?.Message ?? "Невідома помилка при оновленні";
            var backUrl = Url.Action("CreateForm");

            return RedirectToAction("ShowError", "Publication", new
            {
                message = errorMessage,
                returnUrl = backUrl,
            });
        }

        var publications = await mediator.Send(new GetAllPublicationsQuery());
        var parameters = new CvForumViewModel
        {
            Publications = mapper.Map<IEnumerable<PublicationViewModelShort>>(publications.Value),
        };
        return View("~/Views/CvForum/CvForumMainPage.cshtml", parameters);
    }
}