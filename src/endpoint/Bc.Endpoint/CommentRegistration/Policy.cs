using System;
using System.Threading.Tasks;
using Bc.Contracts.Externals.Endpoint.CommentRegistration.Events;
using Bc.Contracts.Externals.Endpoint.ITOps.CreateGitHubPullRequest.Messages;
using Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands;
using NServiceBus;
using NServiceBus.Persistence.Sql;

namespace Bc.Endpoint.CommentRegistration
{
    [SqlSaga(tableSuffix: "RegisterCommentPolicy")]
    public class Policy :
        Saga<Policy.PolicyData>,
        IAmStartedByMessages<RegisterComment>,
        IHandleMessages<ResponseCreatePullRequest>
    {
        
        public Task Handle(RegisterComment message, IMessageHandlerContext context)
        {
            this.Data.UserName = message.UserName;
            this.Data.UserWebsite = message.UserWebsite;
            this.Data.ArticleFileName = message.ArticleFileName;
            this.Data.UserComment = message.UserComment;
            this.Data.CommentAddedDate = message.CommentAddedDate;

            return context.Send(new RequestCreatePullRequest(
                message.CommentId,
                this.Data.UserName,
                this.Data.UserWebsite,
                this.Data.ArticleFileName,
                this.Data.UserComment,
                this.Data.CommentAddedDate));
        }
        
        public Task Handle(ResponseCreatePullRequest message, IMessageHandlerContext context)
        {
            this.MarkAsComplete();
            return context.Publish(new CommentRegistered(this.Data.CommentId, message.PullRequestUri));
        }        
        
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PolicyData> mapper)
        {
            mapper.ConfigureMapping<RequestCreatePullRequest>(msg => msg.CommentId)
                  .ToSaga(data => data.CommentId);
        }

        public class PolicyData : ContainSagaData
        {
            public Guid CommentId { get; set; }

            public string UserName { get; set; }

            public string UserWebsite { get; set; }
            
            public string UserComment { get; set; }

            public string ArticleFileName { get; set; }

            public DateTime CommentAddedDate { get; set; }
        }
    }
}