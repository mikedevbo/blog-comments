using System;

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer
{
    public class CommentRejectedEvt
    {
        public CommentRejectedEvt(Guid commentId)
        {
            this.CommentId = commentId;
        }

        public Guid CommentId { get; }
    }
}