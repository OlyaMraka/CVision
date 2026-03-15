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

using Microsoft.AspNetCore.Mvc;
using CVision.Models.ViewModels.AuthViewModels;


namespace CVision.Controllers
{
    public class AccountController : Controller
    {
        // GET /Account/Login
        // Просто показуємо порожню форму
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LogInViewModel());
        }

        // POST /Account/Login
        // Отримуємо дані форми, перевіряємо ModelState,
        // якщо все ок — перевіряємо логін/пароль через Identity
        [HttpPost]
        [ValidateAntiForgeryToken] // захист від CSRF
        public Task<IActionResult> Login(LogInViewModel model, string? returnUrl = null)
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
        public Task<IActionResult> Register(RegisterViewModel model)
        {
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