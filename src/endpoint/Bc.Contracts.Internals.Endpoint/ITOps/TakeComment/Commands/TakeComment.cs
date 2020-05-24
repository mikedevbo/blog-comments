using System;

namespace Bc.Contracts.Internals.Endpoint.ITOps.TakeComment.Commands
{
    public class TakeComment
    {
        public TakeComment(
            Guid commentId,
            string userName,
            string userEmail,
            string userWebsite,
            string userComment,
            string articleFileName,
            DateTime commentAddedDate)
        {
            this.CommentId = commentId;
            this.UserName = userName;
            this.UserEmail = userEmail;
            this.UserWebsite = userWebsite;
            this.UserComment = userComment;
            this.ArticleFileName = articleFileName;
            this.CommentAddedDate = commentAddedDate;
        }
        
        public Guid CommentId { get; }

        public string UserName { get; }

        public string UserEmail { get; }

        public string UserWebsite { get; }

        public string UserComment { get; }
        
        public string ArticleFileName { get; }

        public DateTime CommentAddedDate { get; }
    }
}