using CVision.BLL.Commands.Users.Login;
using CVision.BLL.Commands.Users.ConfirmEmail;
using CVision.BLL.Commands.Users.Register;
using CVision.BLL.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using CVision.Models.ViewModels.AuthViewModels;
using MediatR;


namespace CVision.Controllers
{
    public class AccountController(IMediator mediator) : Controller
    {
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LogInViewModel());
        }

        [HttpGet]
        public IActionResult EmailConfirm(string? email = null)
        {
            return View("ConfirmEmail", new ConfirmEmailViewModel
            {
                Email = email ?? string.Empty,
            });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int? userId = null, string? token = null)
        {
            bool hasUserId = userId.HasValue;
            bool hasToken = !string.IsNullOrWhiteSpace(token);

            if (!hasUserId && !hasToken)
            {
                return RedirectToAction(nameof(EmailConfirm));
            }

            if (hasUserId ^ hasToken)
            {
                return RedirectToAction(nameof(RegistrationConfirmed), new { isConfirmed = false, errorMessage = "Посилання для підтвердження недійсне." });
            }

            // Підтвердження виконуємо тільки коли є обидва параметри callback-посилання.
            var requestDto = new ConfirmEmailRequestDto
            {
                UserId = userId!.Value,
                Token = token!.Replace(' ', '+'),
            };

            var result = await mediator.Send(new ConfirmEmailCommand(requestDto));

            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(RegistrationConfirmed), new { isConfirmed = true });
            }

            string errorMessage = result.Errors.FirstOrDefault()?.Message
                ?? "Не вдалося підтвердити email. Спробуйте ще раз.";

            return RedirectToAction(nameof(RegistrationConfirmed), new { isConfirmed = false, errorMessage });
        }

        [HttpGet]
        public IActionResult RegistrationConfirmed(bool? isConfirmed = null, string? errorMessage = null)
        {
            // Пряме відкриття без параметрів — редірект на головну
            if (isConfirmed is null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["IsConfirmed"] = isConfirmed.Value;
            ViewData["ConfirmationError"] = errorMessage;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LogInViewModel model, string? returnUrl = null)
        {
            LoginUserRequestDto requestDto = new LoginUserRequestDto()
            {
                Email = model.Email,
                Password = model.Password,
            };
            var response = await mediator.Send(new LoginUserCommand(requestDto));
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

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
            return RedirectToAction(nameof(EmailConfirm), new { email = model.Email });
        }
        [HttpGet]
        public IActionResult Guest()
        {
            return RedirectToAction("hub", "Home");
        }
    }
}