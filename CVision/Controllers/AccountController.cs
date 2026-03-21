using CVision.BLL.Commands.Users.Login;
using CVision.BLL.Commands.Users.ConfirmEmail;
using CVision.BLL.Commands.Users.Register;
using CVision.BLL.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using CVision.Models.ViewModels.AuthViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;
using CVision.DAL.Entities;


namespace CVision.Controllers
{
    public class AccountController(IMediator mediator, SignInManager<ApplicationUser> signInManager) : Controller
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
        public async Task<IActionResult> ConfirmEmail(int userId, string token)
        {
            var requestDto = new ConfirmEmailRequestDto
            {
                UserId = userId,
                Token = token,
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

            if (response.IsSuccess)
            {
                await signInManager.SignInAsync(response.Value, isPersistent: false);
                return RedirectToAction("hub", "Home");
            }

            ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault()?.Message ?? "Невдалось увійти. Перевірте email та пароль.");
            return View(model);
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            RegisterUserRequestDto requestDto = new RegisterUserRequestDto
            {
                UserName = model.UserName,
                Password = model.Password,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
            };

            var response = await mediator.Send(new RegisterUserCommand(requestDto));

            if (response.IsFailed)
            {
                ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault()?.Message ?? "Не вдалося зареєструвати акаунт.");
                return View(model);
            }

            return RedirectToAction(nameof(EmailConfirm), new { email = model.Email });
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Guest()
        {
            return RedirectToAction("hub", "Home");
        }
    }
}