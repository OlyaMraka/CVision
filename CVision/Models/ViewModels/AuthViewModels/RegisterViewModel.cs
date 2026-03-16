using System.ComponentModel.DataAnnotations;

namespace CVision.Models.ViewModels.AuthViewModels
{
    public class RegisterViewModel
    {
        // ── Ім'я ──────────────────────────────────────────────
        [Required(ErrorMessage = "Введіть ваше ім'я")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ім'я: від 2 до 100 символів")]
        public string UserName { get; set; } = string.Empty;

        // ── Email ─────────────────────────────────────────────
        [Required(ErrorMessage = "Введіть електронну пошту")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; } = string.Empty;

        // ── Телефон ───────────────────────────────────────────
        // Phone — вбудований атрибут ASP.NET, перевіряє базовий формат.
        // Regex дозволяє: +380XXXXXXXXX або 0XXXXXXXXX (10–13 цифр з +).
        [Required(ErrorMessage = "Введіть номер телефону")]
        [Phone(ErrorMessage = "Невірний формат телефону")]
        [RegularExpression(
            @"^\+?[0-9\s\-\(\)]{7,20}$",
            ErrorMessage = "Введіть номер у форматі +380 XX XXX XX XX")]
        public string PhoneNumber { get; set; } = string.Empty;

        // ── Пароль ────────────────────────────────────────────
        [Required(ErrorMessage = "Введіть пароль")]
        [MinLength(8, ErrorMessage = "Мінімум 8 символів")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}