namespace Components
{
    using System.Threading.Tasks;
    using Components.GitHub;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus;

    /// <summary>
    /// The message handler.
    /// </summary>
    public class HandlerCreatePullRequest : IHandleMessages<CreatePullRequest>
    {
        private readonly IComponentsConfigurationManager componentsConfigurationManager;
        private readonly IGitHubApi gitHubApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerCreatePullRequest"/> class.
        /// </summary>
        /// <param name="componentsConfigurationManager">The components configuration manager.</param>
        /// <param name="gitHubApi">The git hub API.</param>
        public HandlerCreatePullRequest(IComponentsConfigurationManager componentsConfigurationManager, IGitHubApi gitHubApi)
        {
            this.componentsConfigurationManager = componentsConfigurationManager;
            this.gitHubApi = gitHubApi;
        }

        /// <summary>
        /// Handles a message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="context">The context of the currently handled message.</param>
        /// <returns>Represents the async method.</returns>
        /// <remarks>
        /// This method will be called when a message arrives on at the endpoint and should contain
        /// the custom logic to execute when the message is received.
        /// </remarks>
        public async Task Handle(CreatePullRequest message, IMessageHandlerContext context)
        {
            this.gitHubApi.CreatePullRequest(
                this.componentsConfigurationManager.UserAgent,
                this.componentsConfigurationManager.AuthorizationToken,
                this.componentsConfigurationManager.RepositoryName,
                message.HeadBranchName,
                message.BaseBranchName);

            await context.Publish<IPullRequestCreated>(evt => evt.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }
    }
}
