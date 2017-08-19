namespace Components
{
    using System.Threading.Tasks;
    using Components.GitHub;
    using Messages;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus;

    public class HandlerCheckCommentResponse : IHandleMessages<CheckCommentResponse>
    {
        private readonly IComponentsConfigurationManager componentsConfigurationManager;
        private readonly IGitHubApi gitHubApi;

        public HandlerCheckCommentResponse(IComponentsConfigurationManager componentsConfigurationManager, IGitHubApi gitHubApi)
        {
            this.componentsConfigurationManager = componentsConfigurationManager;
            this.gitHubApi = gitHubApi;
        }

        public async Task Handle(CheckCommentResponse message, IMessageHandlerContext context)
        {
            CommentResponseState responseState = CommentResponseState.Approved;

            var result = await this.gitHubApi.IsPullRequestExists(
                this.componentsConfigurationManager.UserAgent,
                this.componentsConfigurationManager.AuthorizationToken,
                message.PullRequestUri).ConfigureAwait(false);

            if (result)
            {
                responseState = CommentResponseState.NotAddded;
            }
            else
            {
                result = await this.gitHubApi.IsPullRequestMerged(
                    this.componentsConfigurationManager.UserAgent,
                    this.componentsConfigurationManager.AuthorizationToken,
                    message.PullRequestUri).ConfigureAwait(false);

                if (result)
                {
                    responseState = CommentResponseState.Approved;
                }
                else
                {
                    responseState = CommentResponseState.Rejected;
                }
            }

            await context.Publish<ICommentResponseAdded>(
                evt =>
                {
                    evt.CommentId = message.CommentId;
                    evt.CommentResponseState = responseState;
                }).ConfigureAwait(false);
        }
    }
}
