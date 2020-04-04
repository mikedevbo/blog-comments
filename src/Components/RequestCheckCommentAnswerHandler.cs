namespace Components
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Messages;
    using Messages.Messages;
    using NServiceBus;

    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:OpeningParenthesisMustBeSpacedCorrectly", Justification = "Reviewed.")]
    public class RequestCheckCommentAnswerHandler : IHandleMessages<RequestCheckCommentAnswer>
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IGitHubApi gitHubApi;
        private readonly ICommentPolicyLogic logic;

        public RequestCheckCommentAnswerHandler(IConfigurationManager configurationManager, IGitHubApi gitHubApi)
        {
            this.configurationManager = configurationManager;
            this.gitHubApi = gitHubApi;
        }

        public RequestCheckCommentAnswerHandler(IConfigurationManager configurationManager, ICommentPolicyLogic logic)
        {
            this.configurationManager = configurationManager;
            this.logic = logic;
        }

        public async Task Handle(RequestCheckCommentAnswer message, IMessageHandlerContext context)
        {
            //string userAgent = this.configurationManager.UserAgent;
            //string authorizationToken = this.configurationManager.AuthorizationToken;
            //string pullRequestUri = message.PullRequestUri;
            //string etag = message.Etag;

            //CheckCommentAnswerResponse answer = await this.GetCommentAnswer(
            //    () => this.gitHubApi.IsPullRequestOpen(
            //        userAgent,
            //        authorizationToken,
            //        pullRequestUri,
            //        etag),
            //    () => this.gitHubApi.IsPullRequestMerged(
            //        userAgent,
            //        authorizationToken,
            //        pullRequestUri)).ConfigureAwait(false);

            //await context.Reply(answer).ConfigureAwait(false);

            var response = await this.logic.CheckCommentAnswer(message).ConfigureAwait(false);
            await context.Reply(response).ConfigureAwait(false);
        }

        public async Task<CheckCommentAnswerResponse> GetCommentAnswer(
            Func<Task<(bool result, string etag)>> isPullRequestOpen,
            Func<Task<bool>> isPullRequestMerged)
        {
            var answer = new CheckCommentAnswerResponse();

            var isOpen = await isPullRequestOpen().ConfigureAwait(false);
            if (isOpen.result)
            {
                answer.Status = CommentAnswerStatus.NotAddded;
                answer.ETag = isOpen.etag;
                return answer;
            }

            var isMerged = await isPullRequestMerged().ConfigureAwait(false);
            if (isMerged)
            {
                answer.Status = CommentAnswerStatus.Approved;
                answer.ETag = isOpen.etag;
                return answer;
            }

            answer.Status = CommentAnswerStatus.Rejected;
            answer.ETag = isOpen.etag;
            return answer;
        }
    }
}
