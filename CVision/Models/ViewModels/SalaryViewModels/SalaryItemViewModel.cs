namespace CVision.Models.ViewModels.SalaryViewModels;

public class SalaryItemViewModel
{
    public EmployerInfoViewModel Employer { get; set; } = new EmployerInfoViewModel();

    public JobTitleInfoViewModel JobTitle { get; set; } = new JobTitleInfoViewModel();

    public SalaryStatsViewModel BasePayStatistics { get; set; } = new SalaryStatsViewModel();

    public string PayPeriod { get; set; } = string.Empty;

    public PayPercentilesViewModel TotalPayStatistics { get; set; } = new PayPercentilesViewModel();
}