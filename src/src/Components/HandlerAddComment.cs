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
    public class HandlerAddComment : IHandleMessages<AddComment>
    {
        private readonly IComponentsConfigurationManager componentsConfigurationManager;
        private readonly IGitHubApi gitHubApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerAddComment"/> class.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="gitHubApi">The git hub API.</param>
        public HandlerAddComment(IComponentsConfigurationManager configurationManager, IGitHubApi gitHubApi)
        {
            this.componentsConfigurationManager = configurationManager;
            this.gitHubApi = gitHubApi;
        }

        /// <summary>
        /// Handles a message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="context">The context of the currently handled message.</param>
        /// <returns>Representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method will be called when a message arrives on at the endpoint and should contain
        /// the custom logic to execute when the message is received.
        /// </remarks>
        public async Task Handle(AddComment message, IMessageHandlerContext context)
        {
            this.gitHubApi.UpdateFile(
                this.componentsConfigurationManager.UserAgent,
                this.componentsConfigurationManager.AuthorizationToken,
                this.componentsConfigurationManager.RepositoryName,
                message.BranchName,
                message.FileName,
                message.Comment);

            await context.Publish<ICommentAdded>(evt => evt.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }
    }
}
