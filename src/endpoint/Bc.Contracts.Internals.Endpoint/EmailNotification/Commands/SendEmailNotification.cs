using System;

namespace Bc.Contracts.Internals.Endpoint.EmailNotification.Commands
{
    public class SendEmailNotification
    {
        public SendEmailNotification(Guid commentId, string userEmail)
        {
            this.CommentId = commentId;
            this.UserEmail = userEmail;
        }

        public Guid CommentId { get; }

        public string UserEmail { get; }
    }
}