namespace Components
{
    using System;
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
            string userAgent = this.componentsConfigurationManager.UserAgent;
            string authorizationToken = this.componentsConfigurationManager.AuthorizationToken;
            string pullRequestUri = message.PullRequestUri;

            CommentResponseStatus responseStatus = await this.GetCommentResponseStatus(
                () => this.gitHubApi.IsPullRequestOpen(
                    userAgent,
                    authorizationToken,
                    pullRequestUri),
                () => this.gitHubApi.IsPullRequestMerged(
                    userAgent,
                    authorizationToken,
                    pullRequestUri)).ConfigureAwait(false);

            await context.Publish<ICommentResponseAdded>(
                evt =>
                {
                    evt.CommentId = message.CommentId;
                    evt.CommentResponseStatus = responseStatus;
                }).ConfigureAwait(false);
        }

        public async Task<CommentResponseStatus> GetCommentResponseStatus(
            Func<Task<bool>> isPullRequestOpen,
            Func<Task<bool>> isPullRequestMerged)
        {
            if (await isPullRequestOpen().ConfigureAwait(false))
            {
                return CommentResponseStatus.NotAddded;
            }

            if (await isPullRequestMerged().ConfigureAwait(false))
            {
                return CommentResponseStatus.Approved;
            }

            return CommentResponseStatus.Rejected;
        }
    }
}
