using CVision.BLL.DTOs.Contacts;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.Contacts.Search;

public record SearchUsersQuery(int CurrentUserId, string Query, int Limit = 20)
    : IRequest<Result<IEnumerable<UserSearchResultDto>>>;
