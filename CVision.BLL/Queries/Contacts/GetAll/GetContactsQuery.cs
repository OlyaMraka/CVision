using CVision.BLL.DTOs.Contacts;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.Contacts.GetAll;

public record GetContactsQuery(int OwnerId)
    : IRequest<Result<IEnumerable<ContactResponseDto>>>;
