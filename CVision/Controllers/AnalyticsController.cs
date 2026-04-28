using Microsoft.AspNetCore.Mvc;
using CVision.Models.ViewModels.SalaryViewModels;
using MediatR;
using AutoMapper;
using CVision.Helpers.Constants;
using CVision.BLL.DTOs.Analytics;
using CVision.BLL.Queries.Analytics;


namespace CVision.Controllers;

public class AnalyticsController(IMediator mediator, IMapper mapper) : BaseController
{
    [HttpGet]
    public IActionResult SalaryAnalytics()
    {
        return View("~/Views/Salary/Salary.cshtml", new SalaryDataViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SalaryAnalytics(SalaryDataViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.JobTitle) || string.IsNullOrWhiteSpace(model.City))
        {
            return View(model);
        }

        var result = await mediator.Send(new GetMarketAnalyticsQuery(model.JobTitle, model.City));

        if (!result.IsSuccess)
        {
            return ShowError(result.Error ?? SalaryConstants.DataError, Url.Action("SalaryAnalytics")!);
        }

        model.Records = mapper.Map<IEnumerable<SalaryItemViewModel>>(result.Value);

        return View("~/Views/Salary/Salary.cshtml", model);
    }
}