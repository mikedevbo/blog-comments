using System;
using System.Threading.Tasks;
using Bc.Contracts.Externals.Endpoint.CommentAnswer.Events;
using Bc.Contracts.Internals.Endpoint;
using Bc.Contracts.Internals.Endpoint.CommentAnswer;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Messages;
using Bc.Contracts.Internals.Endpoint.ITOps.GitHub;
using Bc.Contracts.Internals.Endpoint.ITOps.GitHub.PullRequestStatusVerification.Messages;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;

namespace Bc.Endpoint.CommentAnswer
{
    [SqlSaga(tableSuffix: "CommentAnswerPolicy")]
    public class Policy :
        Saga<CommentAnswerPolicyData>,
        IAmStartedByMessages<CheckCommentAnswer>,
        IHandleTimeouts<TimeoutCheckCommentAnswer>,
        IHandleMessages<ResponseCheckPullRequestStatus>
    {
        private static readonly ILog Log = LogManager.GetLogger<Policy>();
        private readonly IEndpointConfigurationProvider configurationProvider;

        public Policy(IEndpointConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        }

        public Task Handle(CheckCommentAnswer message, IMessageHandlerContext context)
        {
            this.Data.CommentUri = message.CommentUri;

            return this.RequestTimeout<TimeoutCheckCommentAnswer>(
                context,
                TimeSpan.FromSeconds(this.configurationProvider.CheckCommentAnswerTimeoutInSeconds));
        }

        public Task Timeout(TimeoutCheckCommentAnswer state, IMessageHandlerContext context)
        {
            return context.Send(new RequestCheckPullRequestStatus(this.Data.CommentUri, this.Data.ETag));
        }

        public Task Handle(ResponseCheckPullRequestStatus message, IMessageHandlerContext context)
        {
            var answerStatus = message.PullRequestStatus switch
            {
                PullRequestStatus.Open => CommentAnswerStatus.NotAdded,
                PullRequestStatus.Merged => CommentAnswerStatus.Approved,
                PullRequestStatus.Closed => CommentAnswerStatus.Rejected,
                _ => throw new ArgumentOutOfRangeException($"Not supported pull request status: {message.PullRequestStatus}")
            };

            switch (answerStatus)
            {
                case CommentAnswerStatus.NotAdded:
                    this.Data.ETag = message.ETag;

                    return this.RequestTimeout<TimeoutCheckCommentAnswer>(
                        context,
                        TimeSpan.FromSeconds(this.configurationProvider.CheckCommentAnswerTimeoutInSeconds));

                case CommentAnswerStatus.Approved:
                    this.MarkAsComplete();
                    return context.Publish(new CommentApproved(this.Data.CommentId));

                case CommentAnswerStatus.Rejected:
                    this.MarkAsComplete();
                    return context.Publish(new CommentRejected(this.Data.CommentId));

                default:
                    throw new ArgumentOutOfRangeException($"Not supported comment answer status: {answerStatus}");
            }
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CommentAnswerPolicyData> mapper)
        {
            mapper.ConfigureMapping<CheckCommentAnswer>(message => message.CommentId).ToSaga(data => data.CommentId);
        }
    }

    public class CommentAnswerPolicyData : ContainSagaData
    {
        public Guid CommentId { get; set; }

        public string CommentUri { get; set; }

        public string ETag { get; set; }
    }
}