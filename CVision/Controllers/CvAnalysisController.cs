using MediatR;
using CVision.BLL.Commands.CvAnalyses.Create;
using CVision.BLL.Commands.CvAnalyses.Delete;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.Models.ViewModels.CVAnalysisViewModels;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CVision.BLL.Queries.CvAnalyses.GetAllCvAnalyses;
using CVision.BLL.Queries.CvAnalyses.GetDeletedCvAnalyses;
using Microsoft.AspNetCore.Authorization;

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
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("file", "Будь ласка, завантажте файл");
            return View("CvAnalysisUpload");
        }

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
            return ShowError(result.Error ?? "Помилка аналізу CV", Url.Action("Analyze")!);
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ConfirmDelete(int id)
    {
        // ВАЖЛИВО: Використовуй запит для отримання ОДНОГО CV за його ID
        // Припустимо, він називається GetCvAnalysisByIdQuery
        var result = await mediator.Send(new GetAllByUserIdQuery(id));

        if (!result.IsSuccess)
        {
            return RedirectToAction("CVGallery");
        }

        // Мапимо на спеціальну модель для підтвердження
        var model = mapper.Map<CVAnalysisConfirmationViewModel>(result.Value);

        return View("~/Views/CvAnalysis/CVAnalysisConfirmationPopap.cshtml", model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteCvAnalysisCommand(id));

        if (!result.IsSuccess)
        {
            var errorMessage = result.Error ?? "Не вдалося видалити резюме";
            // Повертаємо на Gallery у разі помилки
            var backUrl = Url.Action("CVGallery", "CvAnalysis")!;
            return ShowError(errorMessage, backUrl);
        }

        return RedirectToAction("CVGallery");
    }
}