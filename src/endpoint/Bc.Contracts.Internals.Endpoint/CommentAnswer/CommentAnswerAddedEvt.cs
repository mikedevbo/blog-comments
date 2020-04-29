using System;

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer
{
    public class CommentAnswerAddedEvt
    {
        public CommentAnswerAddedEvt(Guid commentId, bool isApproved)
        {
            this.CommentId = commentId;
            this.IsApproved = isApproved;
        }

        public Guid CommentId { get; }

        public bool IsApproved { get; }
    }
}