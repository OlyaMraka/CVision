using CVision.BLL.Commands.CvAnalyses.Create;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.Models.ViewModels.CVAnalysisViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;
using CVision.BLL.Queries.CvAnalyses.GetAllCvAnalyses;

namespace CVision.Controllers
{
    [Authorize]
    public class HubController(IMediator mediator, IMapper mapper) : Controller
    {
        [HttpGet]
        public IActionResult Index() => View("~/Views/Hub/CVAnalysis.cshtml");


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Analyze(IFormFile file)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var requestDto = new CreateCvAnalysisRequestDto
            {
                FileStream = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                UserId = userId,
            };

            var result = await mediator.Send(new CreateCvAnalysisCommand(requestDto));


            if (result.IsFailed)
            {
                var error = result.Errors.FirstOrDefault()?.Message
                    ?? "Помилка аналізу CV";

                return BadRequest(new { error });
            }

            var viewModel = mapper.Map<CVAnalysisViewModel>(result.Value);

            return Ok(viewModel);
        }

        [HttpGet]
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
                return View("~/Views/Hub/CVGallery.cshtml", new CVGalleryPageViewModel
                {
                    Items = new List<CVGalleryViewModel>(),
                });
            }


            var vm = new CVGalleryPageViewModel
            {
                Items = mapper.Map<List<CVGalleryViewModel>>(result.Value),
            };

            return View("~/Views/Hub/CVGallery.cshtml", vm);
        }
    }
}