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
    public class HandlerCheckCommentResponse : IHandleMessages<CheckCommentResponse>
    {
        private readonly IComponentsConfigurationManager componentsConfigurationManager;
        private readonly IGitHubApi gitHubApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerCheckCommentResponse"/> class.
        /// </summary>
        /// <param name="componentsConfigurationManager">The components configuration manager.</param>
        /// <param name="gitHubApi">The git hub API.</param>
        public HandlerCheckCommentResponse(IComponentsConfigurationManager componentsConfigurationManager, IGitHubApi gitHubApi)
        {
            this.componentsConfigurationManager = componentsConfigurationManager;
            this.gitHubApi = gitHubApi;
        }

        /// <summary>
        /// Handles a message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="context">The context of the currently handled message.</param>
        /// <returns>Representing the asynchronous operation</returns>
        /// <remarks>
        /// This method will be called when a message arrives on at the endpoint and should contain
        /// the custom logic to execute when the message is received.
        /// </remarks>
        public async Task Handle(CheckCommentResponse message, IMessageHandlerContext context)
        {
            var repo = this.gitHubApi.GetRepository(
                this.componentsConfigurationManager.UserAgent,
                this.componentsConfigurationManager.AuthorizationToken,
                this.componentsConfigurationManager.RepositoryName,
                message.BranchName);

            ////TODO: add if implementation

            await context.Publish<ICommentResponseAdded>(
                evt => {
                    evt.CommentId = message.CommentId;
                    evt.CommentResponseState = Messages.CommentResponseState.Added;
                    })
                .ConfigureAwait(false);
        }
    }
}
