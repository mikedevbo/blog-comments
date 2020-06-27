namespace Web.Models
{
    using FluentValidation;

    public class CommentValidator : AbstractValidator<Comment>
    {
        public CommentValidator()
        {
            const int minLength = 1;
            const int userNameMaxLength = 20;
            const int userWebsiteMaxLength = 30;
            const int contentMaxLength = 4000;

            this.RuleFor(comment => comment.UserName)
                .NotEmpty().WithMessage(WebResource.UserNameNotEmpty)
                .Length(minLength, userNameMaxLength).WithMessage(string.Format(WebResource.UserNameTooLong, userNameMaxLength));

            this.RuleFor(comment => comment.FileName)
                .NotEmpty()
                .WithMessage("File name cannot be empty.");

            this.RuleFor(comment => comment.Content)
                .NotEmpty().WithMessage(WebResource.ContentNotEmpty)
                .Length(minLength, contentMaxLength).WithMessage(string.Format(WebResource.ContentTooLong, contentMaxLength));

            this.RuleFor(comment => comment.UserEmail)
                .EmailAddress().WithMessage(WebResource.UserEmailNotCorrect);

            this.RuleFor(comment => comment.UserWebsite)
                .Length(minLength, userWebsiteMaxLength).WithMessage(string.Format(WebResource.UserWebsiteTooLong, userWebsiteMaxLength));
        }
    }
}