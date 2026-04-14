using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CVision.Helpers.Constants;
using CVision.Models.ViewModels.CVAnalysisViewModels;
using CVision.BLL.Commands.CvAnalyses.Create;
using CVision.BLL.Commands.CvAnalyses.Delete;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Queries.CvAnalyses.GetAllCvAnalyses;

namespace CVision.Controllers;

public class CvAnalysisController(IMediator mediator, IMapper mapper) : BaseController
{
    [HttpGet]
    [Authorize]
    public IActionResult Analyze()
    {
        return View("CvAnalysisUpload");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Analyze(IFormFile file)
    {
        var userId = GetUserId();

        using var stream = file.OpenReadStream();

        var requestDto = new CreateCvAnalysisRequestDto
        {
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            UserId = userId,
        };

        var result = await mediator.Send(new CreateCvAnalysisCommand(requestDto));

        if (!result.IsSuccess)
        {
            return ShowError(result.Error ?? CvAnalysisConstants.AnalyseError, Url.Action("Analyze")!);
        }

        var viewModel = mapper.Map<CVAnalysisViewModel>(result.Value);

        return View("CvAnalysisResult", viewModel);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> CVGallery()
    {
        int userId = GetUserId();

        var result = await mediator.Send(new GetAllByUserIdQuery(userId));

        if (!result.IsSuccess)
        {
            return View("~/Views/CvAnalysis/CVGallery.cshtml", new CVGalleryPageViewModel
            {
                Items = new List<CVGalleryViewModel>(),
            });
        }


        var vm = new CVGalleryPageViewModel
        {
            Items = mapper.Map<List<CVGalleryViewModel>>(result.Value),
        };

        return View("~/Views/CvAnalysis/CVGallery.cshtml", vm);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteCvAnalysisCommand(id));

        if (!result.IsSuccess)
        {
            var errorMessage = result.Error ?? CvAnalysisConstants.DeleteError;
            var backUrl = Url.Action("CVGallery");

            return ShowError(errorMessage, backUrl!);
        }

        return RedirectToAction(nameof(CVGallery));
    }
}
