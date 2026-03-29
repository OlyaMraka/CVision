using AutoMapper;
using CVision.BLL.Commands.CVForum.Create;
using CVision.BLL.DTOs.CVForum;
using CVision.BLL.Queries.CVForum.GetAll;
using CVision.Models.ViewModels.CVForumViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CVision.Controllers
{
    [Authorize]
    public class CVForumController(IMediator mediator, IMapper mapper) : Controller
    {
        // ── GET /CVForum ──────────────────────────────────────
        // Головна сторінка форуму — список всіх постів
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            var result = await mediator.Send(new GetCVForumPostsQuery(userId));

            if (result.IsFailed)
                return View(new CVForumViewModel());

            var viewModel = CVForumViewModel.FromDto(result.Value);
            return View(viewModel);
        }


        // ── POST /CVForum/Create ──────────────────────────────
        // Створення нового поста через форму в модалці.
        // Повертає JSON — JS обробляє відповідь без перезавантаження.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCVForumPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return BadRequest(new { errors });
            }

            var userId = GetUserId();

            var requestDto = new CreateCVForumPostRequestDto
            {
                Title       = model.Title.Trim(),
                Description = model.Description.Trim(),
                FileStream  = model.File.OpenReadStream(),
                FileName    = model.File.FileName,
                ContentType = model.File.ContentType,
                UserId      = userId,
            };

            var result = await mediator.Send(new CreateCVForumPostCommand(requestDto));

            if (result.IsFailed)
                return BadRequest(new {
                    errors = new[] {
                        result.Errors.FirstOrDefault()?.Message ?? "Помилка створення поста"
                    }
                });

            // Повертаємо id нового поста — JS може перенаправити або оновити список
            return Ok(new { success = true, postId = result.Value.Id });
        }


        // ── Хелпер ───────────────────────────────────────────
        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}