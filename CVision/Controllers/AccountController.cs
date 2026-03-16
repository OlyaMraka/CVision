// Controllers/AccountController.cs
// ══════════════════════════════════════════════════════════════
// Контролер для авторизації.
// Маршрути:
//   GET  /Account/Login      → показати форму входу
//   POST /Account/Login      → обробити вхід
//   GET  /Account/Register   → показати форму реєстрації
//   POST /Account/Register   → обробити реєстрацію
//   GET  /Account/Logout     → вийти
// ══════════════════════════════════════════════════════════════

using CVision.BLL.Commands.Users.Register;
using CVision.BLL.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using CVision.Models.ViewModels.AuthViewModels;
using MediatR;


namespace CVision.Controllers
{
    public class AccountController(IMediator mediator) : Controller
    {
        // GET /Account/Login
        // Просто показуємо порожню форму
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LogInViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // захист від CSRF
        public IActionResult Login(LogInViewModel model, string? returnUrl = null)
        {
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            RegisterUserRequestDto requestDto = new RegisterUserRequestDto
            {
                UserName = model.UserName,
                Password = model.Password,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
            };

            var response = await mediator.Send(new RegisterUserCommand(requestDto));
            return View(model);
        }

        // GET /Account/Guest — гостьовий доступ (без реєстрації)
        [HttpGet]
        public IActionResult Guest()
        {
            // Тимчасова сесія гостя або redirect на вибір шаблону
            return RedirectToAction("Index", "Template");
        }

        // GET /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword() => View();
    }
}