using CVision.BLL.Commands.Users.UpdateProfile;
using CVision.BLL.Commands.Users.UpdatePassword;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Users;
using CVision.BLL.Queries.Users.GetUserById;
using CVision.Models.ViewModels.ProfileViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace CVision.Controllers;

public class HomeController(IMediator mediator, ILogger<HomeController> logger) : BaseController
{
    public IActionResult Index() => View();

    [Authorize]
    [ActionName("hub")]
    public IActionResult Hub() => View("hub");

    [HttpGet]
    [Authorize]
    [ActionName("user")]
    public async Task<IActionResult> UserProfile()
    {
        var userId = GetUserId();

        try
        {
            var result = await mediator.Send(new GetUserByIdQuery(userId));
            if (!result.IsSuccess || result.Value == null)
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
            logger.LogError(ex, "Failed to load profile for user with id {UserId}", userId);
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
        var userId = GetUserId();

        var currentUserResult = await mediator.Send(new GetUserByIdQuery(userId));
        var previousEmail = currentUserResult.Value?.Email;

        if (!ModelState.IsValid)
        {
            model.MemberSince = await GetMemberSinceAsync(userId);

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

            var updateResult = await mediator.Send(new UpdateProfileCommand(userId, updateRequestDto));
            if (!updateResult.IsSuccess)
            {
                if (updateResult.Error != null)
                {
                    if (string.Equals(updateResult.Error, UserConstants.EmailAlreadyInUse, StringComparison.Ordinal))
                    {
                        ModelState.AddModelError(nameof(UserWindowViewModel.Email), "Ця електронна пошта вже зайнята.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, updateResult.Error);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Не вдалося оновити профіль. Спробуйте ще раз.");
                }

                model.MemberSince = await GetMemberSinceAsync(userId);

                return View("user", model);
            }

            var emailChanged = !string.IsNullOrWhiteSpace(previousEmail)
                && !string.Equals(previousEmail, model.Email, StringComparison.OrdinalIgnoreCase);

            TempData["UserWindowSuccess"] = emailChanged
                ? "Дані профілю збережено. На нову пошту надіслано лист підтвердження. Підтвердіть email, щоб входити за новою адресою."
                : "Дані профілю збережено.";
            return RedirectToAction("user");
        }
        catch (Exception ex) when (IsEmailSendingNetworkOrTimeoutError(ex))
        {
            logger.LogWarning(ex, "Failed to send confirmation email after profile update for user with id {UserId}", userId);

            var emailChanged = !string.IsNullOrWhiteSpace(previousEmail)
                && !string.Equals(previousEmail, model.Email, StringComparison.OrdinalIgnoreCase);

            if (emailChanged)
            {
                TempData["UserWindowError"] = "Дані профілю збережено, але лист підтвердження нової пошти не вдалося надіслати через проблеми з мережею або таймаут. Перевірте інтернет і спробуйте ще раз пізніше.";
                return RedirectToAction("user");
            }

            TempData["UserWindowError"] = "Не вдалося зберегти зміни через проблеми з мережею або таймаут. Перевірте інтернет і спробуйте ще раз.";
            model.MemberSince = await GetMemberSinceAsync(userId);

            return View("user", model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save profile for user with id {UserId}", userId);
            TempData["UserWindowError"] = "Виникла технічна помилка під час збереження профілю.";
            ModelState.AddModelError(string.Empty, "Не вдалося зберегти зміни. Спробуйте ще раз.");
            model.MemberSince = await GetMemberSinceAsync(userId);

            return View("user", model);
        }
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmNewPassword)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(currentPassword))
        {
            TempData["UserPasswordError"] = "Введіть поточний пароль.";
            return RedirectToAction("user");
        }

        if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmNewPassword))
        {
            TempData["UserPasswordError"] = "Введіть новий пароль та його підтвердження.";
            return RedirectToAction("user");
        }

        if (!string.Equals(newPassword, confirmNewPassword, StringComparison.Ordinal))
        {
            TempData["UserPasswordError"] = "Нові паролі не співпадають.";
            return RedirectToAction("user");
        }

        if (newPassword.Length < UserConstants.MinPasswordLength)
        {
            TempData["UserPasswordError"] = $"Новий пароль має містити щонайменше {UserConstants.MinPasswordLength} символів.";
            return RedirectToAction("user");
        }

        try
        {
            var requestDto = new UpdatePasswordRequestDto
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
            };

            var result = await mediator.Send(new UpdatePasswordCommand(userId, requestDto));
            if (!result.IsSuccess)
            {
                if (result.Error != null && string.Equals(result.Error, UserConstants.IncorrectCurrentPassword, StringComparison.Ordinal))
                {
                    TempData["UserPasswordError"] = "Поточний пароль введено невірно.";
                    return RedirectToAction("user");
                }

                TempData["UserPasswordError"] = result.Error
                    ?? "Не вдалося змінити пароль. Спробуйте ще раз.";
                return RedirectToAction("user");
            }

            TempData["UserPasswordSuccess"] = "Пароль успішно змінено.";
            return RedirectToAction("user");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to change password for user with id {UserId}", userId);
            TempData["UserPasswordError"] = "Виникла технічна помилка під час зміни пароля.";
            return RedirectToAction("user");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();

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

    private async Task<string> GetMemberSinceAsync(int userId)
    {
        var userResult = await mediator.Send(new GetUserByIdQuery(userId));
        return userResult.Value?.CreatedAt.ToString("dd.MM.yyyy") ?? string.Empty;
    }
}
