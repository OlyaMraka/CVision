using CVision.BLL.DTOs.Analytics;

namespace CVision.BLL.Interfaces;

public interface IGlassdoorProvider
{
    Task<IEnumerable<SalaryRecord>> GetMarketAnalyticsAsync(string jobTitle, string city);
}