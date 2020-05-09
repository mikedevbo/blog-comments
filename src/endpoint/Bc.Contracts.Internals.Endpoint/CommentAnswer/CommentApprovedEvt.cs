using System;

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer
{
    public class CommentApprovedEvt
    {
        public CommentApprovedEvt(Guid commentId)
        {
            this.CommentId = commentId;
        }

        public Guid CommentId { get; }
    }
}