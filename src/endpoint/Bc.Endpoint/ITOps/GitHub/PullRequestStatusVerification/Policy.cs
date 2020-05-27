using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.ITOps.GitHub.PullRequestStatusVerification;
using Bc.Contracts.Internals.Endpoint.ITOps.GitHub.PullRequestStatusVerification.Messages;
using NServiceBus;

namespace Bc.Endpoint.ITOps.GitHub.PullRequestStatusVerification
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
                    message.Etag).ConfigureAwait(false);
            
            await context.Reply(response).ConfigureAwait(false);
        }
    }
}