namespace CVision.BLL.DTOs.Vacancies;

public class VacancyDto
{
    public required string Title { get; set; }

    public required string Company { get; set; }

    public required string Url { get; set; }

    public string Source { get; set; } = string.Empty;
}
