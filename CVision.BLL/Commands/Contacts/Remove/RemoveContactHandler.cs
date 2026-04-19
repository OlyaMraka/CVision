using CVision.BLL.Constans;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;

namespace CVision.BLL.Commands.Contacts.Remove;

public class RemoveContactHandler(
    IRepositoryWrapper repositoryWrapper) : IRequestHandler<RemoveContactCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        RemoveContactCommand request,
        CancellationToken cancellationToken)
    {
        var contact = await repositoryWrapper.ContactRepository.GetFirstOrDefaultAsync(
            new QueryOptions<Contact>
            {
                Filter = x => x.OwnerId == request.OwnerId && x.ContactUserId == request.ContactUserId,
                AsNoTracking = false,
            });

        if (contact is null)
        {
            return ContactsConstants.ContactNotFound;
        }

        repositoryWrapper.ContactRepository.Delete(contact);

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return ContactsConstants.DeleteContactError;
        }

        return true;
    }
}
