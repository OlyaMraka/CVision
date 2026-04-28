namespace CVision.Models.ViewModels.SalaryViewModels;

public class SalaryDataViewModel
{
    public string JobTitle { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string ConfirmedLocation { get; set; } = string.Empty;

    public IEnumerable<SalaryItemViewModel> Records { get; set; } = new List<SalaryItemViewModel>();
}