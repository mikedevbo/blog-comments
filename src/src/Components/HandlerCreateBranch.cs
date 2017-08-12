namespace Components
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Components.GitHub;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus;

    /// <summary>
    /// The message handler.
    /// </summary>
    public class HandlerCreateBranch : IHandleMessages<CreateBranch>
    {
        private readonly IComponentsConfigurationManager componentsConfigurationManager;
        private readonly IGitHubApi gitHubApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerCreateBranch"/> class.
        /// </summary>
        /// <param name="componentsConfigurationManager">The components configuration manager.</param>
        /// <param name="gitHubApi">The git hub API.</param>
        public HandlerCreateBranch(IComponentsConfigurationManager componentsConfigurationManager, IGitHubApi gitHubApi)
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
        public async Task Handle(CreateBranch message, IMessageHandlerContext context)
        {
            var sb = new StringBuilder();
            sb.Append(DateTime.UtcNow).Append(Guid.NewGuid());
            string branchName = sb.ToString();

            ////TODO: Is this idempotent ?
            ////TODO: Is this can be awaitable ?
            this.gitHubApi.CreateRepositoryBranch(
                this.componentsConfigurationManager.UserAgent,
                this.componentsConfigurationManager.AuthorizationToken,
                this.componentsConfigurationManager.RepositoryName,
                this.componentsConfigurationManager.MasterBranchName,
                branchName);

            await context.Publish<IBranchCreated>(evt => evt.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }
    }
}
