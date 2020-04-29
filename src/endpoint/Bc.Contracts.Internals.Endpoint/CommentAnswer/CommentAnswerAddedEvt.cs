using System;

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer
{
    public class CommentAnswerAddedEvt
    {
        public CommentAnswerAddedEvt(Guid commentId)
        {
            CommentId = commentId;
        }

        public Guid CommentId { get; }        
    }
}