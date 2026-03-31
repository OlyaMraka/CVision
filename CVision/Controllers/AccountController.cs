using CVision.BLL.Commands.Users.Login;
using CVision.BLL.Commands.Users.ConfirmEmail;
using CVision.BLL.Commands.Users.ForgotPassword;
using CVision.BLL.Commands.Users.Register;
using CVision.BLL.Commands.Users.ResetPassword;
using CVision.BLL.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using CVision.Models.ViewModels.AuthViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;
using CVision.DAL.Entities;


namespace CVision.Controllers
{
    public class AccountController(IMediator mediator, SignInManager<ApplicationUser> signInManager) : BaseController
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

            string errorMessage = TranslateErrorToUkrainian(result.Errors.FirstOrDefault()?.Message)
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

            try
            {
                var response = await mediator.Send(new LoginUserCommand(requestDto));

                if (response.IsSuccess)
                {
                    await signInManager.SignInAsync(response.Value, isPersistent: false);
                    return RedirectToAction("hub", "Home");
                }

                if (!response.Errors.Any())
                {
                    ModelState.AddModelError(string.Empty, "Не вдалося увійти. Перевірте email та пароль.");
                    return View(model);
                }

                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, TranslateErrorToUkrainian(error.Message));
                }

                return View(model);
            }
            catch (Exception ex) when (IsEmailSendingNetworkOrTimeoutError(ex))
            {
                ModelState.AddModelError(string.Empty, "Не вдалося увійти через проблеми з мережею або таймаут. Перевірте інтернет і спробуйте ще раз.");
                return View(model);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Сталася технічна помилка під час входу. Спробуйте ще раз пізніше.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpGet]
        public IActionResult ForgotPassword(string? email = null)
        {
            return View(new ForgotPasswordViewModel
            {
                Email = email ?? string.Empty,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var requestDto = new ForgotPasswordRequestDto
                {
                    Email = model.Email,
                };

                await mediator.Send(new ForgotPasswordCommand(requestDto));
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            catch (Exception ex) when (IsEmailSendingNetworkOrTimeoutError(ex))
            {
                ModelState.AddModelError(string.Empty, "Не вдалося надіслати лист для відновлення через проблеми з мережею або таймаут. Перевірте інтернет і спробуйте ще раз.");
                return View(model);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Сталася технічна помилка під час відновлення пароля. Спробуйте ще раз пізніше.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string? email = null, string? token = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(nameof(Login));
            }

            return View(new ResetPasswordViewModel
            {
                Email = email,
                Token = token,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var requestDto = new ResetPasswordRequestDto
            {
                Email = model.Email,
                Token = model.Token,
                NewPassword = model.NewPassword,
            };

            var result = await mediator.Send(new ResetPasswordCommand(requestDto));

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Пароль успішно змінено. Тепер увійдіть з новим паролем.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, TranslateErrorToUkrainian(error.Message));
            }

            return View(model);
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

            try
            {
                var response = await mediator.Send(new RegisterUserCommand(requestDto));

                if (response.IsFailed)
                {
                    if (!response.Errors.Any())
                    {
                        ModelState.AddModelError(string.Empty, "Не вдалося зареєструвати акаунт.");
                        return View(model);
                    }

                    foreach (var error in response.Errors)
                    {
                        ModelState.AddModelError(string.Empty, TranslateErrorToUkrainian(error.Message));
                    }

                    return View(model);
                }

                return RedirectToAction(nameof(EmailConfirm), new { email = model.Email });
            }
            catch (Exception ex) when (IsEmailSendingNetworkOrTimeoutError(ex))
            {
                ModelState.AddModelError(string.Empty, "Не вдалося надіслати лист підтвердження через проблеми з мережею або таймаут. Перевірте інтернет і спробуйте ще раз.");
                return View(model);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Сталася технічна помилка під час реєстрації. Спробуйте ще раз пізніше.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Guest()
        {
            return RedirectToAction("hub", "Home");
        }
    }
}