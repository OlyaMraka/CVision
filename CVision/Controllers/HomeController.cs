using System.Security.Claims;
using CVision.BLL.Commands.Users.UpdateProfile;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Users;
using CVision.BLL.Queries.Users.GetUserById;
using CVision.Models.ViewModels.ProfileViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CVision.Controllers;

public class HomeController(IMediator mediator, ILogger<HomeController> logger) : Controller
{
    public IActionResult Index() => View();

    [Authorize]
    [ActionName("hub")]
    public IActionResult Hub() => View("hub");

    [Authorize]
    [HttpGet]
    [ActionName("user")]
    public async Task<IActionResult> UserProfile()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var result = await mediator.Send(new GetUserByIdQuery(userId.Value));
            if (result.IsFailed || result.ValueOrDefault == null)
            {
                TempData["UserWindowError"] = "Не вдалося завантажити дані профілю. Спробуйте ще раз.";
                return View("user", new UserWindowViewModel());
            }

            var userDto = result.Value;

            var model = new UserWindowViewModel
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                MemberSince = userDto.CreatedAt.ToString("dd.MM.yyyy"),
            };

            return View("user", model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load profile for user with id {UserId}", userId.Value);
            TempData["UserWindowError"] = "Виникла технічна помилка при завантаженні профілю.";
            return View("user", new UserWindowViewModel());
        }
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("user")]
    public async Task<IActionResult> SaveUserProfile(UserWindowViewModel model)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            model.MemberSince = await GetMemberSinceAsync(userId.Value);

            return View("user", model);
        }

        try
        {
            var updateRequestDto = new UpdateProfileRequestDto
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
            };

            var updateResult = await mediator.Send(new UpdateProfileCommand(userId.Value, updateRequestDto));
            if (updateResult.IsFailed)
            {
                if (!updateResult.Errors.Any())
                {
                    ModelState.AddModelError(string.Empty, "Не вдалося оновити профіль. Спробуйте ще раз.");
                }

                foreach (var error in updateResult.Errors)
                {
                    if (string.Equals(error.Message, UserConstants.EmailAlreadyInUse, StringComparison.Ordinal))
                    {
                        ModelState.AddModelError(nameof(UserWindowViewModel.Email), "Ця електронна пошта вже зайнята.");
                        continue;
                    }

                    ModelState.AddModelError(string.Empty, error.Message);
                }

                model.MemberSince = await GetMemberSinceAsync(userId.Value);

                return View("user", model);
            }

            TempData["UserWindowSuccess"] = "Дані профілю збережено.";
            return RedirectToAction("user");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save profile for user with id {UserId}", userId.Value);
            TempData["UserWindowError"] = "Виникла технічна помилка під час збереження профілю.";
            ModelState.AddModelError(string.Empty, "Не вдалося зберегти зміни. Спробуйте ще раз.");
            model.MemberSince = await GetMemberSinceAsync(userId.Value);

            return View("user", model);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : null;
    }

    private async Task<string> GetMemberSinceAsync(int userId)
    {
        var userResult = await mediator.Send(new GetUserByIdQuery(userId));
        return userResult.ValueOrDefault?.CreatedAt.ToString("dd.MM.yyyy") ?? string.Empty;
    }
}