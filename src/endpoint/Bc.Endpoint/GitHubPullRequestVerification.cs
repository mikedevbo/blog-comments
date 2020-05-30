using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification;
using Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Messages;
using NServiceBus;

namespace Bc.Endpoint
{
    public class Policy : IHandleMessages<RequestCheckPullRequestStatus>
    {
        private readonly IPolicyLogic logic;

        public Policy(IPolicyLogic logic)
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