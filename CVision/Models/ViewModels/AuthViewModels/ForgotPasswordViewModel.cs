using System.ComponentModel.DataAnnotations;

namespace CVision.Models.ViewModels.AuthViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Введіть електронну пошту")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; } = string.Empty;
    }
}
