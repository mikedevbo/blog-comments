namespace Web.Models
{
    using FluentValidation;

    public class CommentValidator : AbstractValidator<Comment>
    {
        public CommentValidator()
        {
            this.RuleFor(comment => comment.UserName)
                .NotEmpty().WithMessage("You must specify a username.")
                .Length(1, 20).WithMessage("Username cannot be longer than 20 characters.");

            this.RuleFor(comment => comment.FileName).NotEmpty();

            this.RuleFor(comment => comment.Content)
                .NotEmpty().WithMessage("You must specify a content.")
                .Length(1, 1000).WithMessage("Comment cannot be longer than 1000 characters.");
        }
    }
}