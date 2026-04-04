using AutoMapper;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Comments;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using FluentValidation;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using MediatR;

namespace CVision.BLL.Commands.Comments.Create;

public class CreateCommentHandler(
    IRepositoryWrapper repositoryWrapper,
    IMapper mapper,
    IValidator<CreateCommentCommand> validator) : IRequestHandler<CreateCommentCommand, Result<CommentResponseDto>>
{
    public async Task<Result<CommentResponseDto>> Handle(
        CreateCommentCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.First().ErrorMessage;
        }

        var newComment = mapper.Map<Comment>(request.RequestDto);

        await repositoryWrapper.CommentRepository.CreateAsync(newComment);

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return CommentsConstants.SaveCommentError;
        }

        newComment.ParentComment = await repositoryWrapper.CommentRepository.GetFirstOrDefaultAsync(
            new QueryOptions<Comment>
            {
                Filter = x => x.Id == newComment.ParentCommentId,
            });

        var response = mapper.Map<CommentResponseDto>(newComment);

        return response;
    }
}