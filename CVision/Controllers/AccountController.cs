using CVision.BLL.Commands.Users.Login;
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
        public IActionResult ConfirmEmail()
        {
            return View(new ConfirmEmailViewModel());
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
            var confirmModel = new ConfirmEmailViewModel { Email = model.Email };
            return View(nameof(ConfirmEmail), confirmModel);
        }
    }
}