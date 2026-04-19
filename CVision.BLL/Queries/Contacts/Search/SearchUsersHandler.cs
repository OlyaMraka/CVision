using CVision.BLL.DTOs.Contacts;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Queries.Contacts.Search;

public class SearchUsersHandler(
    IRepositoryWrapper repositoryWrapper,
    UserManager<ApplicationUser> userManager) : IRequestHandler<SearchUsersQuery, Result<IEnumerable<UserSearchResultDto>>>
{
    public async Task<Result<IEnumerable<UserSearchResultDto>>> Handle(
        SearchUsersQuery request,
        CancellationToken cancellationToken)
    {
        var query = (request.Query ?? string.Empty).Trim().ToLower();

        IQueryable<ApplicationUser> users = userManager.Users
            .Where(u => u.Id != request.CurrentUserId);

        if (!string.IsNullOrEmpty(query))
        {
            users = users.Where(u =>
                (u.UserName != null && u.UserName.ToLower().Contains(query)) ||
                (u.Email != null && u.Email.ToLower().Contains(query)));
        }

        var limit = request.Limit > 0 ? request.Limit : 20;

        var found = await users
            .OrderBy(u => u.UserName)
            .Take(limit)
            .Select(u => new { u.Id, u.UserName, u.Email })
            .ToListAsync(cancellationToken);

        var foundIds = found.Select(u => u.Id).ToList();

        var existingContacts = await repositoryWrapper.ContactRepository.GetAllAsync(
            new QueryOptions<Contact>
            {
                Filter = x => x.OwnerId == request.CurrentUserId && foundIds.Contains(x.ContactUserId),
            });

        var contactIds = existingContacts.Select(c => c.ContactUserId).ToHashSet();

        IEnumerable<UserSearchResultDto> response = found
            .Select(u => new UserSearchResultDto
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                IsContact = contactIds.Contains(u.Id),
            })
            .ToList();

        return Result<IEnumerable<UserSearchResultDto>>.Ok(response);
    }
}
