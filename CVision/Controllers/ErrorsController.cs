using CVision.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVision.Controllers;

[AllowAnonymous]
public class ErrorsController : Controller
{
    [HttpGet("error/{statusCode}")]
    public IActionResult Error(int statusCode)
    {
        string userFriendlyMessage = statusCode switch
        {
            400 => "Здається, ви ввели щось не те. Перевірте дані та спробуйте ще раз.",
            401 => "Ой! Схоже, термін вашої сесії вичерпано. Будь ласка, увійдіть знову.",
            403 => "Вибачте, але у вас немає прав для перегляду цієї сторінки.",
            404 => "Ми шукали всюди, але не змогли знайти таку сторінку в системі CVision.",
            408 => "Сервер занадто довго чекав на відповідь. Перевірте з'єднання.",
            500 => "На сервері щось зламалося. Наші розробники вже отримали сповіщення.",
            _ => "Сталася непередбачувана помилка. Спробуйте оновити сторінку.",
        };

        var viewModel = new ExceptionHandlingViewModel()
        {
            StatusCode = statusCode,
            Message = userFriendlyMessage,
        };

        return View("~/Views/Shared/ExceptionHandlingPage.cshtml", viewModel);
    }
}