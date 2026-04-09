using Microsoft.AspNetCore.Mvc;
using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using CVision.BLL.Queries.CvAnalyses.GetDeletedCvAnalyses;
using CVision.Models.ViewModels.CVBasketViewModels;

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
            // Мапимо результат у колекцію елементів кошика
            Items = mapper.Map<IEnumerable<CVBasketItemViewModel>>(result.Value),
        };

        // Використовуємо прямий шлях до View, як ти зробив у форумі
        return View("~/Views/Basket/Basket.cshtml", model);
    }
}