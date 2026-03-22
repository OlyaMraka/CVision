using CVision.BLL.Commands.CvAnalyses.Create;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.Models.ViewModels.CVAnalysisViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;

namespace CVision.Controllers
{
    [Authorize]
    public class HubController(IMediator mediator, IMapper mapper) : Controller
    {
        // 🔹 GET: /Hub
        [HttpGet]
        public IActionResult Hub() => View("hub");

        // 🔹 POST: /Hub/Analyze
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Analyze(IFormFile file)
        {
            // 1. UserId з Claims
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // 2. DTO
            var requestDto = new CreateCvAnalysisRequestDto
            {
                FileStream = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                UserId = userId,
            };

            // 3. MediatR команда
            var result = await mediator.Send(new CreateCvAnalysisCommand(requestDto));

            // 4. Перевірка результату
            if (result.IsFailed)
            {
                var error = result.Errors.FirstOrDefault()?.Message
                    ?? "Помилка аналізу CV";

                return BadRequest(new { error });
            }

            var viewModel = mapper.Map<CVAnalysisViewModel>(result.Value);
            // 6. JSON для JS
            return Ok(viewModel);
        }
    }
}