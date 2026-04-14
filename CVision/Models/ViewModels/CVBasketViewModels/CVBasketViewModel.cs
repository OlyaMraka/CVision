namespace CVision.Models.ViewModels.CVBasketViewModels;

public class CVBasketViewModel
{
    public IEnumerable<CVBasketItemViewModel> Items { get; init; } = Enumerable.Empty<CVBasketItemViewModel>();
}
