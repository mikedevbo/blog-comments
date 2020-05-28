using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.ITOps.GitHub.PullRequestStatusVerification;
using Bc.Contracts.Internals.Endpoint.ITOps.GitHub.PullRequestStatusVerification.Messages;
using NServiceBus;

namespace Bc.Endpoint
{
    public class GitHubPullRequestStatusVerificationPolicy : IHandleMessages<RequestCheckPullRequestStatus>
    {
        private readonly IPolicyLogic logic;

        public GitHubPullRequestStatusVerificationPolicy(IPolicyLogic logic)
        {
            this.logic = logic;
        }
        
        public async Task Handle(RequestCheckPullRequestStatus message, IMessageHandlerContext context)
        {
            var response = await this.logic.CheckPullRequestStatus(
                    message.PullRequestUri,
                    message.Etag).ConfigureAwait(false);
            
            await context.Reply(response).ConfigureAwait(false);
        }
    }
}