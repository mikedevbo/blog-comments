using System;

namespace Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands
{
    public class RegisterComment
    {
        public RegisterComment(
            Guid commentId,
            string userName,
            string userWebsite,
            string userComment,
            string articleFileName,
            DateTime commentAddedDate)
        {
            this.CommentId = commentId;
            this.UserName = userName;
            this.UserWebsite = userWebsite;
            this.UserComment = userComment;
            this.ArticleFileName = articleFileName;
            this.CommentAddedDate = commentAddedDate;
        }

        public Guid CommentId { get;  }

        public string UserName { get;  }

        public string UserWebsite { get;  }
        
        public string UserComment { get;  }

        public string ArticleFileName { get;  }

        public DateTime CommentAddedDate { get;  }
    }
}