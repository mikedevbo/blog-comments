using System;

namespace Bc.Contracts.Internals.Endpoint.EmailNotification.Commands
{
    public class NotifyAnswerByEmail
    {
        public NotifyAnswerByEmail(Guid commentId)
        {
            this.CommentId = commentId;
        }

        public Guid CommentId { get; }
        
    }
}