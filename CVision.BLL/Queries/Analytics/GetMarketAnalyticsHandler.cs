using MediatR;
using CVision.BLL.Helpers;
using CVision.BLL.DTOs.Analytics;
using CVision.BLL.Interfaces;

namespace CVision.BLL.Queries.Analytics;

public class GetMarketAnalyticsHandler(IGlassdoorProvider glassdoorProvider)
    : IRequestHandler<GetMarketAnalyticsQuery, Result<IEnumerable<SalaryRecord>>>
{
    public async Task<Result<IEnumerable<SalaryRecord>>> Handle(
        GetMarketAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.JobTitle) || string.IsNullOrWhiteSpace(request.City))
        {
            return "Job title and city are required.";
        }

        var data = await glassdoorProvider.GetMarketAnalyticsAsync(request.JobTitle, request.City);

        var salaryRecords = data as List<SalaryRecord> ?? data.ToList();

        if (!salaryRecords.Any())
        {
            return "No analytics data found for the specified criteria.";
        }

        var sortedData = salaryRecords.OrderByDescending(r => r.BasePayStatistics?.Mean);

        return Result<IEnumerable<SalaryRecord>>.Ok(sortedData);
    }
}