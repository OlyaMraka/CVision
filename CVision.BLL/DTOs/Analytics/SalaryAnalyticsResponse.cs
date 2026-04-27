namespace CVision.BLL.DTOs.Analytics;

public class SalaryAnalyticsResponse
{
    public SalaryData Data { get; set; } = new SalaryData();

    public bool Status { get; set; }
}