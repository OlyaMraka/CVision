using AutoMapper;
using CVision.BLL.DTOs.Contacts;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Queries.Contacts.GetAll;

public class GetContactsHandler(
    IMapper mapper,
    IRepositoryWrapper repositoryWrapper) : IRequestHandler<GetContactsQuery, Result<IEnumerable<ContactResponseDto>>>
{
    public async Task<Result<IEnumerable<ContactResponseDto>>> Handle(
        GetContactsQuery request,
        CancellationToken cancellationToken)
    {
        var contacts = await repositoryWrapper.ContactRepository.GetAllAsync(
            new QueryOptions<Contact>
            {
                Filter = x => x.OwnerId == request.OwnerId,
                Include = x => x.Include(c => c.ContactUser),
            });

        var response = mapper.Map<IEnumerable<ContactResponseDto>>(contacts)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return response;
    }
}
