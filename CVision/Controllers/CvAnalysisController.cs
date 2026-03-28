using MediatR;
using CVision.BLL.Commands.CvAnalyses.Create;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.Models.ViewModels.CVAnalysisViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;
using CVision.BLL.Queries.CvAnalyses.GetAllCvAnalyses;

namespace CVision.Controllers;

public class CvAnalysisController(IMediator mediator, IMapper mapper) : Controller
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
            ModelState.AddModelError("file", "Будь ласка, завантажте файл");
            return View("CvAnalysisUpload");
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        using var stream = file.OpenReadStream();

        var requestDto = new CreateCvAnalysisRequestDto
        {
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            UserId = userId,
        };

        var result = await mediator.Send(new CreateCvAnalysisCommand(requestDto));

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message ?? "Помилка аналізу CV";
            ModelState.AddModelError(string.Empty, errorMessage);

            return View("CvAnalysisUpload");
        }

        var viewModel = mapper.Map<CVAnalysisViewModel>(result.Value);

        return View("CvAnalysisResult", viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> CVGallery()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        int userId = int.Parse(userIdClaim);


        var result = await mediator.Send(new GetAllByUserIdQuery(userId));


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