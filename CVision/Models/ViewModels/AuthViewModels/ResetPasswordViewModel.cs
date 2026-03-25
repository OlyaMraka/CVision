using System.ComponentModel.DataAnnotations;

namespace CVision.Models.ViewModels.AuthViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Введіть електронну пошту")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Токен відновлення відсутній")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть новий пароль")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Пароль має містити щонайменше 8 символів")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Підтвердіть новий пароль")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
