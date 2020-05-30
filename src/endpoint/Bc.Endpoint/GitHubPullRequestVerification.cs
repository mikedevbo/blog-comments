using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Logic;
using Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Messages;
using NServiceBus;

namespace Bc.Endpoint
{
    public class GitHubPullRequestVerificationPolicy : IHandleMessages<RequestCheckPullRequestStatus>
    {
        private readonly IGitHubPullRequestVerificationPolicyLogic logic;

        public GitHubPullRequestVerificationPolicy(IGitHubPullRequestVerificationPolicyLogic logic)
        {
            this.logic = logic;
        }
        
        public async Task Handle(RequestCheckPullRequestStatus message, IMessageHandlerContext context)
        {
            var response = await this.logic.CheckPullRequestStatus(
                    message.PullRequestUri,
                    message.ETag).ConfigureAwait(false);
            
            await context.Reply(response).ConfigureAwait(false);
        }
    }
}