using System.Security.Claims;
using CVision.BLL.Queries.Users.GetUserById;
using CVision.Models.ViewModels.ProfileViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVision.Controllers;

public class HomeController(IMediator mediator) : Controller
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

        var result = await mediator.Send(new GetUserByIdQuery(userId.Value));

        if (result.IsFailed)
        {
            return RedirectToAction("Index");
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

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("user")]
    public async Task<IActionResult> SaveUserProfile(UserWindowViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var userId = GetCurrentUserId();
            if (userId != null)
            {
                var userResult = await mediator.Send(new GetUserByIdQuery(userId.Value));
                model.MemberSince = userResult.ValueOrDefault?.CreatedAt.ToString("dd.MM.yyyy") ?? string.Empty;
            }

            return View("user", model);
        }

        TempData["UserWindowSuccess"] = "Дані профілю збережено.";
        return RedirectToAction("user");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : null;
    }
}