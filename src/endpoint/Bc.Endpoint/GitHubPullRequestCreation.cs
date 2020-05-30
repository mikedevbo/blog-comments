using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.GitHubPullRequestCreation;
using Bc.Contracts.Internals.Endpoint.GitHubPullRequestCreation.Messages;
using NServiceBus;
using NServiceBus.Persistence.Sql;

namespace Bc.Endpoint.GitHubPullRequestCreation
{
    //[SqlSaga(tableSuffix: "GitHubPullRequestCreationPolicy")]
    public class Policy :
        Saga<Policy.PolicyData>,
        IAmStartedByMessages<RequestCreateGitHubPullRequest>,
        IHandleMessages<ResponseCreateBranch>,
        IHandleMessages<ResponseUpdateFile>,
        IHandleMessages<ResponseCreatePullRequest>
    {
        public Task Handle(RequestCreateGitHubPullRequest message, IMessageHandlerContext context)
        {
            this.Data.UserName = message.UserName;
            this.Data.UserWebSite = message.UserWebsite;
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
            return this.ReplyToOriginator(
                context,
                new ResponseCreateGitHubPullRequest(this.Data.CommentId, message.PullRequestUri));
        }
        
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PolicyData> mapper)
        {
            mapper.ConfigureMapping<RequestCreateGitHubPullRequest>(message => message.CommentId)
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
    
    public class PolicyHandlers :
        IHandleMessages<RequestCreateBranch>,
        IHandleMessages<RequestUpdateFile>,
        IHandleMessages<RequestCreatePullRequest>
    {
        private readonly IPolicyLogic logic;

        public PolicyHandlers(IPolicyLogic logic)
        {
            this.logic = logic;
        }        
        
        public async Task Handle(RequestCreateBranch message, IMessageHandlerContext context)
        {
            var branchName = await this.logic.CreateBranch(message.AddedDate).ConfigureAwait(false);
            await context.Reply(new ResponseCreateBranch(branchName)).ConfigureAwait(false);
        }

        public async Task Handle(RequestUpdateFile message, IMessageHandlerContext context)
        {
            await this.logic.UpdateFile(message.BranchName, message.FileName, message.Content).ConfigureAwait(false);
            await context.Reply(new ResponseUpdateFile()).ConfigureAwait(false);
        }

        public async Task Handle(RequestCreatePullRequest message, IMessageHandlerContext context)
        {
            var pullRequestUri = await this.logic.CreatePullRequest(message.BranchName).ConfigureAwait(false);
            await context.Reply(new ResponseCreatePullRequest(pullRequestUri)).ConfigureAwait(false);
        }
    }    
}