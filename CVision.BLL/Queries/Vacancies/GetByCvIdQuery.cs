using CVision.BLL.DTOs.Vacancies;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.Vacancies;

public record GetByCvIdQuery(int CvId)
    : IRequest<Result<IEnumerable<VacancyDto>>>;
