namespace Web.Modules
{
    using System;
    using FluentValidation;
    using Messages.Commands;
    using Nancy;
    using Nancy.ModelBinding;
    using NServiceBus;
    using Web.Models;

    public class CommentModule : NancyModule
    {
        private readonly IMessageSession messageSession;
        private readonly IValidator validator;

        public CommentModule(IMessageSession messageSession, IValidator validator)
            : base("/comment")
        {
            this.messageSession = messageSession;
            this.validator = validator;

            this.Get["/"] = r => "test";

            this.Post["/", true] = async (r, c) =>
            {
                var comment = this.Bind<Comment>();

                var validationResult = await this.validator.ValidateAsync(comment).ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    return this.Negotiate.WithModel(validationResult).WithStatusCode(HttpStatusCode.BadRequest);
                }

                await this.messageSession.Send<StartAddingComment>(msg =>
                {
                    msg.CommentId = Guid.NewGuid();
                    msg.UserName = comment.UserName;
                    msg.UserEmail = comment.UserEmail;
                    msg.UserWebsite = comment.UserWebsite;
                    msg.FileName = comment.FileName;
                    msg.Content = comment.Content;
                    msg.AddedDate = DateTime.UtcNow;
                }).ConfigureAwait(false);

                return HttpStatusCode.OK;
            };
        }
    }
}