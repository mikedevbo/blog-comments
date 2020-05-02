using System;

namespace Bc.Contracts.Internals.Endpoint.EmailNotification
{
    public class NotifyAnswerByEmailCmd
    {
        public NotifyAnswerByEmailCmd(Guid commentId, bool isApproved)
        {
            this.CommentId = commentId;
            this.IsApproved = isApproved;
        }

        public Guid CommentId { get; }

        public bool IsApproved { get; }
    }
}