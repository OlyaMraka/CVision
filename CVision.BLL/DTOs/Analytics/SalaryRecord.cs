namespace CVision.BLL.DTOs.Analytics;

public class SalaryRecord
{
    public EmployerInfo Employer { get; set; } = new EmployerInfo();

    public JobTitleInfo JobTitle { get; set; } = new JobTitleInfo();

    public string PayPeriod { get; set; } = string.Empty;

    public SalaryStats BasePayStatistics { get; set; } = new SalaryStats();

    public PayPercentiles TotalPayStatistics { get; set; } = new PayPercentiles();
}