using System;
using System.Threading.Tasks;
using Externals = Bc.Contracts.Externals.Endpoint.ITOps.CreateGitHubPullRequest.Messages;
using Bc.Contracts.Internals.Endpoint.ITOps.CreateGitHubPullRequest.Messages;
using NServiceBus;
using NServiceBus.Persistence.Sql;

namespace Bc.Endpoint.ITOps.CreateGitHubPullRequest
{
    [SqlSaga(tableSuffix: "CreateGitHubPullRequest")]
    public class Policy :
        Saga<Policy.PolicyData>,
        IAmStartedByMessages<Externals.RequestCreatePullRequest>,
        IHandleMessages<ResponseCreateBranch>,
        IHandleMessages<ResponseUpdateFile>,
        IHandleMessages<ResponseCreatePullRequest>
    {
        public Task Handle(Externals.RequestCreatePullRequest message, IMessageHandlerContext context)
        {
            this.Data.UserName = message.UserName;
            this.Data.UserWebSite = message.UserWebSite;
            this.Data.FileName = message.FileName;
            this.Data.Content = message.Content;
            this.Data.AddedDate = message.AddedDate;

            return context.Send(new RequestCreateBranch(this.Data.AddedDate));
        }

        public Task Handle(ResponseCreateBranch message, IMessageHandlerContext context)
        {
            this.Data.BranchName = message.BranchName;
            return context.Send(new RequestUpdateFile(this.Data.BranchName, this.Data.FileName, this.Data.Content));
        }

        public Task Handle(ResponseUpdateFile message, IMessageHandlerContext context)
        {
            return context.Send(new RequestCreatePullRequest(this.Data.BranchName));
        }

        public Task Handle(ResponseCreatePullRequest message, IMessageHandlerContext context)
        {
            this.MarkAsComplete();
            return this.ReplyToOriginator(context, new Externals.ResponseCreatePullRequest(message.PullRequestUri));
        }
        
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PolicyData> mapper)
        {
            mapper.ConfigureMapping<Externals.RequestCreatePullRequest>(msg => msg.CommentId)
                  .ToSaga(data => data.CommentId);
        }        

        public class PolicyData : ContainSagaData
        {
            public Guid CommentId { get; set; }

            public string UserName { get; set; }

            public string UserWebSite { get; set; }

            public string FileName { get; set; }

            public string Content { get; set; }

            public DateTime AddedDate { get; set; }

            public string BranchName { get; set; }    
        }
    }
}