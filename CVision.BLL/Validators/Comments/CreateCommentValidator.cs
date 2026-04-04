using FluentValidation;
using CVision.BLL.Commands.Comments.Create;
using CVision.BLL.Constans;

namespace CVision.BLL.Validators.Comments;

public class CreateCommentValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.RequestDto.Content)
            .NotEmpty().WithMessage(CommentsConstants.CommentContentRequired)
            .MaximumLength(CommentsConstants.MaxCommentLength).WithMessage(CommentsConstants.CommentMaxLenghtError)
            .MinimumLength(CommentsConstants.MinCommentLength).WithMessage(CommentsConstants.CommentMinLenghtError);
    }
}