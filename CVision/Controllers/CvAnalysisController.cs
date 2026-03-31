using MediatR;
using CVision.BLL.Commands.CvAnalyses.Create;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.Models.ViewModels.CVAnalysisViewModels;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CVision.BLL.Queries.CvAnalyses.GetAllCvAnalyses;

namespace CVision.Controllers;

public class CvAnalysisController(IMediator mediator, IMapper mapper) : BaseController
{
    [HttpGet]
    public IActionResult Analyze()
    {
        return View("CvAnalysisUpload");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Analyze(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("file", FileIsRequiredError);
            return View("CvAnalysisUpload");
        }

        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return RedirectToLogin();
        }

        using var stream = file.OpenReadStream();

        var requestDto = new CreateCvAnalysisRequestDto
        {
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            UserId = userId.Value,
        };

        var result = await mediator.Send(new CreateCvAnalysisCommand(requestDto));

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message ?? CvAnalysisFailedError;
            ModelState.AddModelError(string.Empty, errorMessage);

            return View("CvAnalysisUpload");
        }

        var viewModel = mapper.Map<CVAnalysisViewModel>(result.Value);

        return View("CvAnalysisResult", viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> CVGallery()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return RedirectToLogin();
        }

        var result = await mediator.Send(new GetAllByUserIdQuery(userId.Value));


        if (result.IsFailed)
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
}