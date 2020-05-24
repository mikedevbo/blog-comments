using System;

namespace Bc.Contracts.Internals.Endpoint.ITOps.Commands
{
    public class TakeComment
    {
        public TakeComment(
            Guid commentId,
            string userName,
            string userEmail,
            string userWebsite,
            string fileName,
            string content,
            DateTime addedDate)
        {
            this.CommentId = commentId;
            this.UserName = userName;
            this.UserEmail = userEmail;
            this.UserWebsite = userWebsite;
            this.FileName = fileName;
            this.Content = content;
            this.AddedDate = addedDate;
        }
        
        public Guid CommentId { get; }

        public string UserName { get; }

        public string UserEmail { get; }

        public string UserWebsite { get; }

        public string FileName { get; }

        public string Content { get; }

        public DateTime AddedDate { get; }        
    }
}