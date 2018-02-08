namespace Components
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Messages;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus;

    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:OpeningParenthesisMustBeSpacedCorrectly", Justification = "Reviewed.")]
    public class HandlerCheckCommentResponse : IHandleMessages<CheckCommentResponse>
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IGitHubApi gitHubApi;

        public HandlerCheckCommentResponse(IConfigurationManager configurationManager, IGitHubApi gitHubApi)
        {
            this.configurationManager = configurationManager;
            this.gitHubApi = gitHubApi;
        }

        public async Task Handle(CheckCommentResponse message, IMessageHandlerContext context)
        {
            string userAgent = this.configurationManager.UserAgent;
            string authorizationToken = this.configurationManager.AuthorizationToken;
            string pullRequestUri = message.PullRequestUri;
            string etag = message.Etag;

            CommentResponse response = await this.GetCommentResponseStatus(
                () => this.gitHubApi.IsPullRequestOpen(
                    userAgent,
                    authorizationToken,
                    pullRequestUri,
                    etag),
                () => this.gitHubApi.IsPullRequestMerged(
                    userAgent,
                    authorizationToken,
                    pullRequestUri)).ConfigureAwait(false);

            await context.Publish<ICommentResponseAdded>(
                evt =>
                {
                    evt.CommentId = message.CommentId;
                    evt.CommentResponse = response;
                }).ConfigureAwait(false);
        }

        public async Task<CommentResponse> GetCommentResponseStatus(
            Func<Task<(bool result, string etag)>> isPullRequestOpen,
            Func<Task<bool>> isPullRequestMerged)
        {
            var response = new CommentResponse();

            var isOpen = await isPullRequestOpen().ConfigureAwait(false);
            if (isOpen.result)
            {
                response.ResponseStatus = CommentResponseStatus.NotAddded;
                response.ETag = isOpen.etag;
                return response;
            }

            var isMerged = await isPullRequestMerged().ConfigureAwait(false);
            if (isMerged)
            {
                response.ResponseStatus = CommentResponseStatus.Approved;
                response.ETag = isOpen.etag;
                return response;
            }

            response.ResponseStatus = CommentResponseStatus.Rejected;
            response.ETag = isOpen.etag;
            return response;
        }
    }
}
