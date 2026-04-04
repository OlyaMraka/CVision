using CVision.BLL.Commands.Comments.Update;
using CVision.BLL.Constans;
using FluentValidation;

namespace CVision.BLL.Validators.Comments;

public class UpdateCommentValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentValidator()
    {
        RuleFor(x => x.RequestDto.Content)
            .NotEmpty().WithMessage(CommentsConstants.CommentContentRequired)
            .MaximumLength(CommentsConstants.MaxCommentLength).WithMessage(CommentsConstants.CommentMaxLenghtError)
            .MinimumLength(CommentsConstants.MinCommentLength).WithMessage(CommentsConstants.CommentMinLenghtError);
    }
}
