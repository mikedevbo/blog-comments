using System;

namespace Bc.Contracts.Internals.Endpoint.CommentRegistration
{
    public class RegisterCommentCmd
    {
        public RegisterCommentCmd(CommentData commentData)
        {
            CommentData = commentData;
        }

        public CommentData CommentData { get; }
    }
}