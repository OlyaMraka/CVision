using AutoMapper;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Comments;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using FluentValidation;
using MediatR;

namespace CVision.BLL.Commands.Comments.Update;

public class UpdateCommentHandler(
    IRepositoryWrapper repositoryWrapper,
    IMapper mapper,
    IValidator<UpdateCommentCommand> validator) : IRequestHandler<UpdateCommentCommand, Result<CommentResponseDto>>
{
    public async Task<Result<CommentResponseDto>> Handle(
        UpdateCommentCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.First().ErrorMessage;
        }

        var comment = await repositoryWrapper.CommentRepository.GetFirstOrDefaultAsync(
            new QueryOptions<Comment>
            {
                Filter = x => x.Id == request.RequestDto.Id,
            });

        if (comment is null)
        {
            return CommentsConstants.CommentNotFound;
        }

        comment.Content = request.RequestDto.Content;

        repositoryWrapper.CommentRepository.Update(comment);

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return CommentsConstants.SaveCommentError;
        }

        var response = mapper.Map<CommentResponseDto>(comment);

        return response;
    }
}
