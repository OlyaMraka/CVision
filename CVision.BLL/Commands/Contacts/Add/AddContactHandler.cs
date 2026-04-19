using AutoMapper;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Contacts;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Commands.Contacts.Add;

public class AddContactHandler(
    IRepositoryWrapper repositoryWrapper,
    IMapper mapper,
    UserManager<ApplicationUser> userManager) : IRequestHandler<AddContactCommand, Result<ContactResponseDto>>
{
    public async Task<Result<ContactResponseDto>> Handle(
        AddContactCommand request,
        CancellationToken cancellationToken)
    {
        if (request.OwnerId == request.ContactUserId)
        {
            return ContactsConstants.CannotAddSelfAsContact;
        }

        var targetUser = await userManager.FindByIdAsync(request.ContactUserId.ToString());
        if (targetUser is null)
        {
            return ContactsConstants.TargetUserNotFound;
        }

        var existing = await repositoryWrapper.ContactRepository.GetFirstOrDefaultAsync(
            new QueryOptions<Contact>
            {
                Filter = x => x.OwnerId == request.OwnerId && x.ContactUserId == request.ContactUserId,
            });

        if (existing is not null)
        {
            return ContactsConstants.ContactAlreadyExists;
        }

        var newContact = new Contact
        {
            OwnerId = request.OwnerId,
            ContactUserId = request.ContactUserId,
        };

        await repositoryWrapper.ContactRepository.CreateAsync(newContact);

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return ContactsConstants.SaveContactError;
        }

        var saved = await repositoryWrapper.ContactRepository.GetFirstOrDefaultAsync(
            new QueryOptions<Contact>
            {
                Filter = x => x.Id == newContact.Id,
                Include = x => x.Include(c => c.ContactUser),
            });

        return mapper.Map<ContactResponseDto>(saved);
    }
}
