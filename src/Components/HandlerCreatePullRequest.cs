namespace Components
{
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus;

    public class HandlerCreatePullRequest : IHandleMessages<CreatePullRequest>
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IGitHubApi gitHubApi;

        public HandlerCreatePullRequest(IConfigurationManager configurationManager, IGitHubApi gitHubApi)
        {
            this.configurationManager = configurationManager;
            this.gitHubApi = gitHubApi;
        }

        public async Task Handle(CreatePullRequest message, IMessageHandlerContext context)
        {
            var result = await this.gitHubApi.CreatePullRequest(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                this.configurationManager.RepositoryName,
                message.CommentBranchName,
                message.BaseBranchName).ConfigureAwait(false);

            await context.Publish<IPullRequestCreated>(evt =>
            {
                evt.CommentId = message.CommentId;
                evt.PullRequestLocation = result;
            })
            .ConfigureAwait(false);
        }
    }
}
