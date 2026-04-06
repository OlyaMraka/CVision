using CVision.BLL.Constans;
using CVision.BLL.Helpers;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using FluentValidation;
using MediatR;

namespace CVision.BLL.Commands.Publications.Update;

public class UpdatePublicationHandler(
    IRepositoryWrapper repositoryWrapper,
    IValidator<UpdatePublicationCommand> validator) : IRequestHandler<UpdatePublicationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UpdatePublicationCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.Errors.First().ErrorMessage;
        }

        var publication = await repositoryWrapper.PublicationRepository.GetFirstOrDefaultAsync(
            new QueryOptions<DAL.Entities.Publication>
            {
                Filter = p => p.Id == request.PublicationId,
                AsNoTracking = false,
            });

        if (publication is null)
        {
            return PublicationsConstants.PublicationNotFound;
        }

        if (publication.UserId != request.UserId)
        {
            return "Unauthorized";
        }

        publication.Title = request.RequestDto.Title;
        publication.Description = request.RequestDto.Description;

        repositoryWrapper.PublicationRepository.Update(publication);

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return PublicationsConstants.PublicationUpdateError;
        }

        return true;
    }
}
