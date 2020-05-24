using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.ITOps.CreateGitHubPullRequest;
using Bc.Contracts.Internals.Endpoint.ITOps.CreateGitHubPullRequest.Messages;
using NServiceBus;

namespace Bc.Endpoint.ITOps.CreateGitHubPullRequest
{
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