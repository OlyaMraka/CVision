using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CVision.Models.ViewModels.CvForum;

namespace CVision.Controllers;

public class BaseController : Controller
{
    public int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    public IActionResult ShowError(string message, string returnUrl)
    {
        var model = new ErrorWindowViewModal
        {
            Message = message,
            ReturnUrl = returnUrl,
        };

        return View("~/Views/Shared/GeneralError.cshtml", model);
    }
}