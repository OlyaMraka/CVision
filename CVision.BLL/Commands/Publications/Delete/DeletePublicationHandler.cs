using CVision.BLL.Constans;
using CVision.BLL.Helpers;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;

namespace CVision.BLL.Commands.Publications.Delete;

public class DeletePublicationHandler(
    IRepositoryWrapper repositoryWrapper) : IRequestHandler<DeletePublicationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
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
            return PublicationsConstants.PublicationNotFound;
        }

        if (publication.UserId != request.UserId)
        {
            return "Unauthorized";
        }

        repositoryWrapper.PublicationRepository.Delete(publication);

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return PublicationsConstants.PublicationDeleteError;
        }

        return true;
    }
}
