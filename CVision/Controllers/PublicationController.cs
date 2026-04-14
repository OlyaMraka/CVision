using AutoMapper;
using CVision.BLL.Commands.Publications.Create;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.DTOs.Comments;
using CVision.BLL.Queries.Publications.GetAllPublications;
using CVision.Models.ViewModels.CvForum;
using MediatR;
using System.Security.Claims;
using CVision.BLL.Commands.Comments.Create;
using CVision.BLL.Commands.Comments.Delete;
using CVision.BLL.Commands.Comments.ToggleReaction;
using CVision.BLL.Commands.Comments.Update;
using CVision.BLL.Commands.Publications.Delete;
using CVision.BLL.Commands.Publications.Update;
using CVision.BLL.Queries.Publications.GetByPublicationId;
using CVision.BLL.Queries.Publications.GetByUserId;
using CVision.BLL.Queries.Comments.GetByPublicationId;
using CVision.DAL.Entities;
using CVision.Helpers.Constants;
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
        var pubResult = await mediator.Send(new GetPublicationByIdQuery(publicationId));
        if (!pubResult.IsSuccess)
        {
            return RedirectToAction("CvForum");
        }

        var parameter = mapper.Map<PublicationsViewModel>(pubResult.Value);
        parameter.IsOwn = userId == parameter.UserId;
        var commentsResult = await mediator.Send(new GetByPublicationIdQuery(publicationId, userId));
        if (commentsResult.IsSuccess)
        {
            ViewBag.Comments = mapper.Map<IEnumerable<CommentViewModel>>(commentsResult.Value);
        }
        else
        {
            ViewBag.Comments = new List<CommentViewModel>();
        }

        return View("~/Views/CvForum/PublicationPage.cshtml", parameter);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePublication(int publicationId)
    {
        var userId = GetUserId();

        var result = await mediator.Send(new DeletePublicationCommand(publicationId, userId));

        if (!result.IsSuccess)
        {
            var errorMessage = result.Error ?? PublicationConstants.DeletePublicationError;
            var backUrl = Url.Action("GetPublication", "Publication", new { publicationId })
                ?? Url.Action("CvForum", "Publication")!;

            return ShowError(errorMessage, backUrl);
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

        if (!result.IsSuccess)
        {
            var errorMessage = result.Error ?? PublicationConstants.UpdatePublicationError;
            var backUrl = Url.Action("GetPublication", "Publication", new { publicationId = model.Id })
                ?? Url.Action("CvForum", "Publication")!;

            return ShowError(errorMessage, backUrl);
        }

        return RedirectToAction("GetPublication", new { publicationId = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePublication(IFormFile file, string title, string description)
    {
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

        if (!result.IsSuccess)
        {
            var errorMessage = result.Error ?? PublicationConstants.CreatePublicationError;
            var backUrl = Url.Action("CreateForm", "Publication")
                ?? Url.Action("CvForum", "Publication")!;

            return ShowError(errorMessage, backUrl);
        }

        return RedirectToAction("CvForum");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(CreateCommentViewModel model)
    {
        var userId = GetUserId();

        var requestDto = new CreateCommentRequestDto
        {
            PublicationId = model.PublicationId,
            UserId = userId,
            ParentCommentId = model.ParentCommentId,
            Content = model.Content,
        };

        var result = await mediator.Send(new CreateCommentCommand(requestDto));

        if (!result.IsSuccess)
        {
            var errorMessage = result.Error ?? CommentConstants.AddCommentError;
            var backUrl = Url.Action("GetPublication", "Publication", new { publicationId = model.PublicationId })
                ?? Url.Action("CvForum", "Publication")!;

            return ShowError(errorMessage, backUrl);
        }

        return RedirectToAction("GetPublication", new { publicationId = model.PublicationId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditComment(int commentId, int publicationId, string content)
    {
        var requestDto = new UpdateCommentRequestDto
        {
            Id = commentId,
            Content = content,
        };

        var result = await mediator.Send(new UpdateCommentCommand(requestDto));

        if (!result.IsSuccess)
        {
            var errorMessage = result.Error ?? CommentConstants.UpdateCommentError;
            var backUrl = Url.Action("GetPublication", "Publication", new { publicationId })
                ?? Url.Action("CvForum", "Publication")!;

            return ShowError(errorMessage, backUrl);
        }

        return RedirectToAction("GetPublication", new { publicationId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(int commentId, int publicationId)
    {
        var result = await mediator.Send(new DeleteCommentCommand(commentId));

        if (!result.IsSuccess)
        {
            var errorMessage = result.Error ?? CommentConstants.DeleteCommentError;
            var backUrl = Url.Action("GetPublication", "Publication", new { publicationId })
                ?? Url.Action("CvForum", "Publication")!;

            return ShowError(errorMessage, backUrl);
        }

        return RedirectToAction("GetPublication", new { publicationId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCommentReaction(int commentId, int publicationId, ReactionType reactionType)
    {
        var userId = GetUserId();

        var result = await mediator.Send(new ToggleReactionCommand(commentId, userId, reactionType));

        if (!result.IsSuccess)
        {
            var errorMessage = result.Error ?? CommentConstants.ToggleReactionError;
            var backUrl = Url.Action("GetPublication", "Publication", new { publicationId })
                ?? Url.Action("CvForum", "Publication")!;

            return ShowError(errorMessage, backUrl);
        }

        return RedirectToAction("GetPublication", new { publicationId });
    }
}
