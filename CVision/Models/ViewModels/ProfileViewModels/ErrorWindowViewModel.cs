namespace CVision.Models.ViewModels.ProfileViewModels
{
    public class ErrorWindowViewModel
    {
        public string? UserWindowError { get; set; }

        public string? PasswordWindowError { get; set; }

        public bool HasErrors =>
            !string.IsNullOrWhiteSpace(UserWindowError) ||
            !string.IsNullOrWhiteSpace(PasswordWindowError);
    }
}
