using System;

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer.Events
{
    public class CommentAnswerAdded
    {
        public CommentAnswerAdded(Guid commentId, bool isApproved)
        {
            this.CommentId = commentId;
            this.IsApproved = isApproved;
        }

        public Guid CommentId { get; }

        public bool IsApproved { get; }
    }
}