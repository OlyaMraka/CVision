using System.ComponentModel.DataAnnotations;

namespace CVision.Models.ViewModels.AuthViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введіть ваше ім'я")]
        [StringLength(100, ErrorMessage = "Ім'я занадто довге")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть електронну пошту")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пароль")]
        [MinLength(8, ErrorMessage = "Мінімум 8 символів")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        // Чекбокс "Погоджуюсь з Умовами"
        [Range(typeof(bool), "true", "true", ErrorMessage = "Необхідно погодитись з умовами")]
        public bool AgreeToTerms { get; set; }
    }
}