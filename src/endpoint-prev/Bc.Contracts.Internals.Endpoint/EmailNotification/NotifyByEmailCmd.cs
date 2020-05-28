using System;

namespace Bc.Contracts.Internals.Endpoint.EmailNotification
{
    public class NotifyByEmailCmd
    {
        public NotifyByEmailCmd(Guid commentId, string userEmail)
        {
            this.CommentId = commentId;
            this.UserEmail = userEmail;
        }

        public Guid CommentId { get; }

        public string UserEmail { get; }        
    }
}