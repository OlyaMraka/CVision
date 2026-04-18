using CVision.BLL.DTOs.Vacancies;

namespace CVision.BLL.Interfaces;

public interface IVacancyProvider
{
    Task<ICollection<VacancyDto>> SearchJobs(string query);
}