using System.Linq;
using System.Collections.Generic;
using CVision.BLL.DTOs.Analytics;
using CVision.BLL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CVision.BLL.Services;

public class GlassdoorProvider(IHttpClientService httpService, IConfiguration config) : IGlassdoorProvider
{
    private readonly string _apiKey = config["RapidApi:Key"] ?? throw new Exception("RapidAPI Key is missing");
    private readonly string _apiHost = "glassdoor-real-time.p.rapidapi.com";

    public async Task<IEnumerable<SalaryRecord>> GetMarketAnalyticsAsync(string jobTitle, string city)
    {
        var locationUrl = $"https://{_apiHost}/salaries/location?query={Uri.EscapeDataString(city)}";
        var locData = await httpService.GetFromJsonAsync<GlassdoorLocationResponse>(locationUrl, GetHeaders());

        var locationId = locData?.Data?.FirstOrDefault()?.LocationId;
        if (string.IsNullOrEmpty(locationId))
        {
            return Enumerable.Empty<SalaryRecord>();
        }

        var salaryUrl = $"https://{_apiHost}/salaries/search?query={Uri.EscapeDataString(jobTitle)}&locationId={locationId}";
        var response = await httpService.GetFromJsonAsync<SalaryAnalyticsResponse>(salaryUrl, GetHeaders());

        var results = response?.Data?.AggregateSalaryResponse?.Results ?? new List<SalaryRecord>();

        foreach (var record in results.Where(r => r.PayPeriod.Equals("ANNUAL")))
        {
            if (record.BasePayStatistics != null)
            {
                record.BasePayStatistics.Mean /= 12;
            }

            if (record.TotalPayStatistics?.Percentiles != null)
            {
                foreach (var percentile in record.TotalPayStatistics.Percentiles)
                {
                    percentile.Value /= 12;
                }
            }

            record.PayPeriod = "MONTHLY";
        }

        return results;
    }

    private Dictionary<string, string> GetHeaders() => new()
    {
        { "x-rapidapi-key", _apiKey },
        { "x-rapidapi-host", _apiHost },
    };
}