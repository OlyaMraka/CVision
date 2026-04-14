using AutoMapper;
using CVision.BLL.DTOs.Comments;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CVision.BLL.Queries.Comments.GetByPublicationId;

public class GetByPublicationIdHandler(
    IMapper mapper,
    IRepositoryWrapper repositoryWrapper) : IRequestHandler<GetByPublicationIdQuery, Result<IEnumerable<CommentResponseDto>>>
{
    public async Task<Result<IEnumerable<CommentResponseDto>>> Handle(
        GetByPublicationIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<Comment>
        {
            Filter = x => x.PublicationId == request.PublicationId && !x.IsDeleted,
            Include = x =>
                x.Include(x => x.User)
                    .Include(x => x.ParentComment)
                    .ThenInclude(x => x!.User)
                    .Include(x => x.Reactions),
        };

        var comments = await repositoryWrapper.CommentRepository.GetAllAsync(queryOptions);

        var response = mapper.Map<IEnumerable<CommentResponseDto>>(comments)
            .OrderBy(x => x.CreatedAt)
            .ToList();

        if (request.CurrentUserId.HasValue)
        {
            var commentsList = comments.ToList();
            foreach (var dto in response)
            {
                var comment = commentsList.FirstOrDefault(c => c.Id == dto.Id);
                var userReaction = comment?.Reactions
                    .FirstOrDefault(r => r.UserId == request.CurrentUserId.Value);
                dto.CurrentUserReaction = userReaction?.ReactionType;
            }
        }

        return response;
    }
}