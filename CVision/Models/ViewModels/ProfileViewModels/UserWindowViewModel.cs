using System.ComponentModel.DataAnnotations;

namespace CVision.Models.ViewModels.ProfileViewModels
{
    public class UserWindowViewModel
    {
        [Required(ErrorMessage = "Введіть ваше ім'я")]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "Ім'я має містити від 4 до 40 символів")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть електронну пошту")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "Email має містити від 4 до 40 символів")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть номер телефону")]
        [Phone(ErrorMessage = "Невірний формат телефону")]
        [RegularExpression(
            @"^\+?[0-9\s\-\(\)]{7,20}$",
            ErrorMessage = "Введіть номер у форматі +380 XX XXX XX XX")]
        public string PhoneNumber { get; set; } = string.Empty;

        public string MemberSince { get; set; } = string.Empty;

        public string StatusLabel { get; set; } = "Active profile";
    }
}