using System;

namespace Bc.Contracts.Internals.Endpoint.EmailNotification
{
    public class NotifyAnswerByEmailCmd
    {
        public NotifyAnswerByEmailCmd(Guid commentId)
        {
            this.CommentId = commentId;
        }

        public Guid CommentId { get; }
    }
}