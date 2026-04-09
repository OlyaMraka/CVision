namespace CVision.Models.ViewModels.CVBasketViewModels;

public class CVBasketItemViewModel
{
    public int Id { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public int Days { get; set; }

    public string DeletionNotice => $"Видалено. Залишилося днів: {Days}";
}