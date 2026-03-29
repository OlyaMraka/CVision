using CVision.BLL.Constans;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using FluentResults;
using FluentValidation;
using MediatR;

namespace CVision.BLL.Commands.Publications.Update;

public class UpdatePublicationHandler(
    IRepositoryWrapper repositoryWrapper,
    IValidator<UpdatePublicationCommand> validator) : IRequestHandler<UpdatePublicationCommand, Result>
{
    public async Task<Result> Handle(
        UpdatePublicationCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.First().ErrorMessage);
        }

        var publication = await repositoryWrapper.PublicationRepository.GetFirstOrDefaultAsync(
            new QueryOptions<DAL.Entities.Publication>
            {
                Filter = p => p.Id == request.PublicationId,
                AsNoTracking = false,
            });

        if (publication is null)
        {
            return Result.Fail(PublicationsConstants.PublicationNotFound);
        }

        if (publication.UserId != request.UserId)
        {
            return Result.Fail("Unauthorized");
        }

        publication.Title = request.RequestDto.Title;
        publication.Description = request.RequestDto.Description;

        repositoryWrapper.PublicationRepository.Update(publication);

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return Result.Fail(PublicationsConstants.PublicationUpdateError);
        }

        return Result.Ok();
    }
}
