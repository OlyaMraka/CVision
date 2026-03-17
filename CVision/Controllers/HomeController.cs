using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CVision.Models;
using System.Text.Json;
using CVision.DAL.Entities;
using CVision.Models.ViewModels.ProfileViewModels;
using Microsoft.AspNetCore.Identity;


namespace CVision.Controllers
{
    public class HomeController(UserManager<ApplicationUser> userManager) : Controller
    {
        private const string UserProfileSessionKey = "CVision.UserWindowProfile";

        public IActionResult Index()
        {
            return View();
        }

        [ActionName("hub")]
        public IActionResult Hub()
        {
            return View("hub");
        }

        [HttpGet]
        [ActionName("user")]
        public async Task<IActionResult> UserProfile()
        {
            UserWindowViewModel model = await BuildUserWindowViewModelAsync();
            return View("user", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("user")]
        public async Task<IActionResult> SaveUserProfile(UserWindowViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.MemberSince = await GetMemberSinceAsync();
                return View("user", model);
            }

            ApplicationUser? user = await userManager.GetUserAsync(User);
            if (user is not null)
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                IdentityResult result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    model.MemberSince = FormatMemberSince(user.CreatedAt);
                    return View("user", model);
                }
            }

            SaveProfileToSession(model);
            TempData["UserWindowSuccess"] = "Дані профілю збережено.";
            return RedirectToAction("user");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        private async Task<UserWindowViewModel> BuildUserWindowViewModelAsync()
        {
            ApplicationUser? user = await userManager.GetUserAsync(User);
            if (user is not null)
            {
                return new UserWindowViewModel
                {
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    MemberSince = FormatMemberSince(user.CreatedAt),
                };
            }

            UserWindowViewModel? storedProfile = LoadProfileFromSession();
            if (storedProfile is not null)
            {
                return storedProfile;
            }

            return new UserWindowViewModel
            {
                UserName = "Alex CVision",
                Email = "alex.cvision@example.com",
                PhoneNumber = "+380 67 123 45 67",
                MemberSince = FormatMemberSince(DateTime.UtcNow.AddMonths(-2)),
            };
        }

        private async Task<string> GetMemberSinceAsync()
        {
            ApplicationUser? user = await userManager.GetUserAsync(User);
            if (user is not null)
            {
                return FormatMemberSince(user.CreatedAt);
            }

            UserWindowViewModel? storedProfile = LoadProfileFromSession();
            return storedProfile?.MemberSince ?? FormatMemberSince(DateTime.UtcNow.AddMonths(-2));
        }

        private UserWindowViewModel? LoadProfileFromSession()
        {
            string? storedJson = HttpContext.Session.GetString(UserProfileSessionKey);
            if (string.IsNullOrWhiteSpace(storedJson))
            {
                return null;
            }

            UserWindowViewModel? model = JsonSerializer.Deserialize<UserWindowViewModel>(storedJson);
            if (model is not null && string.IsNullOrWhiteSpace(model.MemberSince))
            {
                model.MemberSince = FormatMemberSince(DateTime.UtcNow.AddMonths(-2));
            }

            return model;
        }

        private void SaveProfileToSession(UserWindowViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.MemberSince))
            {
                model.MemberSince = FormatMemberSince(DateTime.UtcNow.AddMonths(-2));
            }

            string json = JsonSerializer.Serialize(model);
            HttpContext.Session.SetString(UserProfileSessionKey, json);
        }

        private string FormatMemberSince(DateTime createdAt)
        {
            return createdAt.ToLocalTime().ToString("dd.MM.yyyy");
        }
    }
}