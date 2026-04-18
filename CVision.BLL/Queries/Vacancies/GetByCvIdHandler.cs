using CVision.BLL.DTOs.Vacancies;
using CVision.BLL.Helpers;
using CVision.BLL.Interfaces;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;

namespace CVision.BLL.Queries.Vacancies;

public class GetByCvIdHandler(
    IRepositoryWrapper repositoryWrapper,
    IVacancyProvider vacancyProvider) : IRequestHandler<GetByCvIdQuery, Result<IEnumerable<VacancyDto>>>
{
    public async Task<Result<IEnumerable<VacancyDto>>> Handle(
        GetByCvIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<CvLookup>
        {
            Filter = x => x.CvId == request.CvId,
        };

        var keyWords = await repositoryWrapper.CvLookupRepository.GetAllAsync(queryOptions);

        List<VacancyDto> result = new List<VacancyDto>();

        foreach (var keyWord in keyWords)
        {
            var vacancies = await vacancyProvider.SearchJobs(keyWord.LookupWord);
            result.AddRange(vacancies.ToList());
        }

        return Result<IEnumerable<VacancyDto>>.Ok(result);
    }
}
