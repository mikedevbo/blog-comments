namespace Components
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Messages;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus;

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

            CommentResponse responseStatus = await this.GetCommentResponseStatus(
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
                    evt.CommentResponse = responseStatus;
                    //evt.ETag = 
                }).ConfigureAwait(false);
        }

        public async Task<CommentResponse> GetCommentResponseStatus(
            Func<Task<bool>> isPullRequestOpen,
            Func<Task<bool>> isPullRequestMerged)
        {
            var response = new CommentResponse();

            if (await isPullRequestOpen().ConfigureAwait(false))
            {
                response.ResponseStatus = CommentResponseStatus.NotAddded;
            }
            else if (await isPullRequestMerged().ConfigureAwait(false))
            {
                response.ResponseStatus = CommentResponseStatus.Approved;
            }
            else
            {
                response.ResponseStatus = CommentResponseStatus.Rejected;
            }

            return response;
        }
    }
}
