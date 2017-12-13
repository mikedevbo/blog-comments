namespace Web.Modules
{
    using System;
    using Messages.Commands;
    using Nancy;
    using Nancy.ModelBinding;
    using NServiceBus;
    using Web.Models;

    public class CommentModule : NancyModule
    {
        private readonly IMessageSession messageSession;

        public CommentModule(IMessageSession messageSession)
            : base("/comment")
        {
            this.messageSession = messageSession;

            this.Get["/"] = r => "test";

            this.Post["/", true] = async (r, c) =>
            {
                var comment = this.Bind<Comment>();
                await this.messageSession.Send<StartAddingComment>(command =>
                {
                    command.CommentId = Guid.NewGuid();
                    command.UserName = comment.UserName;
                    command.UserEmail = comment.UserEmail;
                    command.UserWebsite = comment.UserWebsite;
                    command.FileName = comment.FileName;
                    command.Content = comment.Content;
                }).ConfigureAwait(false);

                return HttpStatusCode.OK;
            };
        }
    }
}