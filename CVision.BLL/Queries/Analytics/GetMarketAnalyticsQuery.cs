using MediatR;
using CVision.BLL.Helpers;
using CVision.BLL.DTOs.Analytics;

namespace CVision.BLL.Queries.Analytics;

public record GetMarketAnalyticsQuery(string JobTitle, string City)
    : IRequest<Result<IEnumerable<SalaryRecord>>>;
