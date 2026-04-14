using CVision.Helpers.Constants;
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
            400 => ErorrWindowConstants.Error400,
            401 => ErorrWindowConstants.Error401,
            403 => ErorrWindowConstants.Error403,
            404 => ErorrWindowConstants.Error404,
            408 => ErorrWindowConstants.Error408,
            500 => ErorrWindowConstants.Error500,
            _ => ErorrWindowConstants.UnknownError,
        };

        var viewModel = new ExceptionHandlingViewModel()
        {
            StatusCode = statusCode,
            Message = userFriendlyMessage,
        };

        return View("~/Views/Shared/ExceptionHandlingPage.cshtml", viewModel);
    }
}
