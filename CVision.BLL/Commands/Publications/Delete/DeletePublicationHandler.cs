using CVision.BLL.Constans;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using FluentResults;
using MediatR;

namespace CVision.BLL.Commands.Publications.Delete;

public class DeletePublicationHandler(
    IRepositoryWrapper repositoryWrapper) : IRequestHandler<DeletePublicationCommand, Result>
{
    public async Task<Result> Handle(
        DeletePublicationCommand request,
        CancellationToken cancellationToken)
    {
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

        repositoryWrapper.PublicationRepository.Delete(publication);

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return Result.Fail(PublicationsConstants.PublicationDeleteError);
        }

        return Result.Ok();
    }
}
