using System.Security.Claims;
using System.Net.Http;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;

namespace CVision.Controllers;

public abstract class BaseController : Controller
{
    protected const string FileIsRequiredError = "Будь ласка, завантажте файл";

    protected const string CvAnalysisFailedError = "Помилка аналізу CV";

    protected static bool IsEmailSendingNetworkOrTimeoutError(Exception exception)
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

    protected static string TranslateErrorToUkrainian(string? message)
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

    protected int? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var userId) ? userId : null;
    }

    protected IActionResult RedirectToLogin() => RedirectToAction("Login", "Account");
}
