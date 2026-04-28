namespace CVision.Models.ViewModels.SalaryViewModels;

public class EmployerInfoViewModel
{
    public string Name { get; set; } = string.Empty;

    public string SquareLogoUrl { get; set; } = string.Empty;

    public EmployerRatingsViewModel Ratings { get; set; } = new EmployerRatingsViewModel();
}