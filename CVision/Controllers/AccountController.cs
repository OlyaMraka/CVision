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
using System.Net.Mail;


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

            string errorMessage = TranslateErrorToUkrainian(result.Error)
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
                    await signInManager.SignInAsync(response.Value!, isPersistent: false);
                    return RedirectToAction("tips", "Home");
                }

                if (response.Error == null)
                {
                    ModelState.AddModelError(string.Empty, "Не вдалося увійти. Перевірте email та пароль.");
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, TranslateErrorToUkrainian(response.Error));

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

            if (result.Error != null)
            {
                ModelState.AddModelError(string.Empty, TranslateErrorToUkrainian(result.Error));
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

                if (!response.IsSuccess)
                {
                    if (response.Error == null)
                    {
                        ModelState.AddModelError(string.Empty, "Не вдалося зареєструвати акаунт.");
                        return View(model);
                    }

                    ModelState.AddModelError(string.Empty, TranslateErrorToUkrainian(response.Error));

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
            return RedirectToAction("tips", "Home");
        }

        private static bool IsEmailSendingNetworkOrTimeoutError(Exception exception)
        {
            Exception? current = exception;
            while (current is not null)
            {
                if (current is TimeoutException
                    || current is TaskCanceledException
                    || current is HttpRequestException
                    || current is SmtpException)
                {
                    return true;
                }

                current = current.InnerException;
            }

            return false;
        }

        private static string TranslateErrorToUkrainian(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return "Сталася помилка. Спробуйте ще раз.";
            }

            bool hasAlreadyConflictWord = message.Contains("already", StringComparison.OrdinalIgnoreCase)
                || message.Contains("taken", StringComparison.OrdinalIgnoreCase)
                || message.Contains("in use", StringComparison.OrdinalIgnoreCase)
                || message.Contains("exists", StringComparison.OrdinalIgnoreCase);

            bool isEmailConflict = message.Contains("email", StringComparison.OrdinalIgnoreCase)
                && hasAlreadyConflictWord;

            bool isUserNameConflict = (message.Contains("user name", StringComparison.OrdinalIgnoreCase)
                    || message.Contains("username", StringComparison.OrdinalIgnoreCase)
                    || message.Contains("user", StringComparison.OrdinalIgnoreCase))
                && hasAlreadyConflictWord;

            if (isUserNameConflict)
            {
                return "Користувач з таким іменем вже існує.";
            }

            if (isEmailConflict)
            {
                return "Користувач з такою електронною поштою вже існує.";
            }

            if (string.Equals(message, "A user with this information already exists!", StringComparison.Ordinal)
                || string.Equals(message, "This email is already in use!", StringComparison.Ordinal))
            {
                return "Користувач з такими даними вже існує.";
            }

            if (string.Equals(message, "Incorrect login or password!", StringComparison.Ordinal))
            {
                return "Невірний email або пароль.";
            }

            if (string.Equals(message, "Email is not confirmed!", StringComparison.Ordinal))
            {
                return "Електронну пошту ще не підтверджено. Підтвердіть email перед входом.";
            }

            if (string.Equals(message, "Email is required!", StringComparison.Ordinal))
            {
                return "Введіть електронну пошту.";
            }

            if (string.Equals(message, "Email must be shorter than 40 characters!", StringComparison.Ordinal))
            {
                return "Email має містити не більше 40 символів.";
            }

            if (string.Equals(message, "Email must be longer than 4 characters!", StringComparison.Ordinal))
            {
                return "Email має містити щонайменше 4 символи.";
            }

            if (string.Equals(message, "Username is required!", StringComparison.Ordinal))
            {
                return "Введіть ім'я користувача.";
            }

            if (string.Equals(message, "Username must be shorter than 40 characters!", StringComparison.Ordinal))
            {
                return "Ім'я має містити не більше 40 символів.";
            }

            if (string.Equals(message, "Username must be longer than 4 characters!", StringComparison.Ordinal))
            {
                return "Ім'я має містити щонайменше 4 символи.";
            }

            if (string.Equals(message, "Password is required!", StringComparison.Ordinal))
            {
                return "Введіть пароль.";
            }

            if (string.Equals(message, "Password must be longer than 8 characters!", StringComparison.Ordinal)
                || message.Contains("Passwords must be at least", StringComparison.OrdinalIgnoreCase))
            {
                return "Пароль має містити щонайменше 8 символів.";
            }

            if (string.Equals(message, "Password must contain at least one uppercase letter!", StringComparison.Ordinal)
                || message.Contains("uppercase", StringComparison.OrdinalIgnoreCase))
            {
                return "Пароль має містити щонайменше одну велику літеру.";
            }

            if (message.Contains("lowercase", StringComparison.OrdinalIgnoreCase))
            {
                return "Пароль має містити щонайменше одну малу літеру.";
            }

            if (string.Equals(message, "Password must contain at least one digit!", StringComparison.Ordinal)
                || message.Contains("digit", StringComparison.OrdinalIgnoreCase))
            {
                return "Пароль має містити щонайменше одну цифру.";
            }

            if (string.Equals(message, "Password must contain special characters!", StringComparison.Ordinal)
                || message.Contains("non alphanumeric", StringComparison.OrdinalIgnoreCase))
            {
                return "Пароль має містити щонайменше один спеціальний символ.";
            }

            if (string.Equals(message, "User not found!", StringComparison.Ordinal))
            {
                return "Користувача не знайдено.";
            }

            if (string.Equals(message, "Email confirmation failed!", StringComparison.Ordinal))
            {
                return "Не вдалося підтвердити email.";
            }

            if (string.Equals(message, "Current password is incorrect!", StringComparison.Ordinal))
            {
                return "Поточний пароль введено невірно.";
            }

            if (string.Equals(message, "Failed to change password!", StringComparison.Ordinal))
            {
                return "Не вдалося змінити пароль.";
            }

            if (string.Equals(message, "Password reset failed! The token may be invalid or expired.", StringComparison.Ordinal))
            {
                return "Не вдалося скинути пароль. Посилання недійсне або вже прострочене.";
            }

            if (message.Contains("invalid token", StringComparison.OrdinalIgnoreCase))
            {
                return "Недійсний токен відновлення. Спробуйте запросити новий лист.";
            }

            if (string.Equals(message, "Failed to update profile!", StringComparison.Ordinal))
            {
                return "Не вдалося оновити профіль.";
            }

            return message;
        }
    }
}
