namespace CVision.Models.ViewModels.SalaryViewModels;

public class PayPercentilesViewModel
{
    public ICollection<SalaryPercentileViewModel> Percentiles { get; set; } = new List<SalaryPercentileViewModel>();
}