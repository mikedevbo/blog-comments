namespace Components
{
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Messages.Messages;
    using NServiceBus;

    public class RequestCreatePullRequestHandler : IHandleMessages<RequestCreatePullRequest>
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IGitHubApi gitHubApi;

        public RequestCreatePullRequestHandler(IConfigurationManager configurationManager, IGitHubApi gitHubApi)
        {
            this.configurationManager = configurationManager;
            this.gitHubApi = gitHubApi;
        }

        public async Task Handle(RequestCreatePullRequest message, IMessageHandlerContext context)
        {
            var result = await this.gitHubApi.CreatePullRequest(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                this.configurationManager.RepositoryName,
                message.CommentBranchName,
                message.BaseBranchName).ConfigureAwait(false);

            await context.Reply<CreatePullRequestResponse>(response =>
            {
                response.PullRequestLocation = result;
            })
            .ConfigureAwait(false);
        }
    }
}
