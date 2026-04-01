using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CVision.Controllers;

public class BaseController : Controller
{
    public int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}