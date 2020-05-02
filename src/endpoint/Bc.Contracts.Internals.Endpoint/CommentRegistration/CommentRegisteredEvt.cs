using System;

namespace Bc.Contracts.Internals.Endpoint.CommentRegistration
{
    public class CommentRegisteredEvt
    {
        public CommentRegisteredEvt(Guid commentId, string commentUri)
        {
            this.CommentId = commentId;
            this.CommentUri = commentUri;
        }

        public Guid CommentId { get; }

        public string CommentUri { get; }        
    }
}