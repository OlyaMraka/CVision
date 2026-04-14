using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CVision.Models.ViewModels.CVBasketViewModels;
using CVision.BLL.Queries.CvAnalyses.GetDeletedCvAnalyses;

namespace CVision.Controllers;

[Authorize]
public class BasketController(IMediator mediator, IMapper mapper) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();

        var result = await mediator.Send(new GetDeletedByUserIdQuery(userId));

        var model = new CVBasketViewModel
        {
            Items = mapper.Map<IEnumerable<CVBasketItemViewModel>>(result.Value),
        };
        return View("~/Views/Basket/Basket.cshtml", model);
    }
}
