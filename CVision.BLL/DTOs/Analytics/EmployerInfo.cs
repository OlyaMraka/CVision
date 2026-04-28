namespace CVision.BLL.DTOs.Analytics;

public class EmployerInfo
{
    public string Name { get; set; } = string.Empty;

    public string SquareLogoUrl { get; set; } = string.Empty;

    public EmployerRatings Ratings { get; set; } = new EmployerRatings();
}
