using System;
using System.Threading.Tasks;
using Bc.Contracts.Externals.Endpoint.CommentAnswer.Events;
using Bc.Contracts.Internals.Endpoint.CommentAnswer;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Logic;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Messages;
using Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification;
using Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Messages;
using NServiceBus;

namespace Bc.Endpoint
{
    //[SqlSaga(tableSuffix: "CommentAnswerPolicy")]
    public class CommentAnswerPolicy :
        Saga<CommentAnswerPolicy.PolicyData>,
        IAmStartedByMessages<CheckCommentAnswer>,
        IHandleMessages<ResponseCheckPullRequestStatus>,
        IHandleTimeouts<TimeoutCheckCommentAnswer>
    {
        private readonly ICommentAnswerPolicyLogic logic;

        public CommentAnswerPolicy(ICommentAnswerPolicyLogic logic)
        {
            this.logic = logic;
        }

        public Task Handle(CheckCommentAnswer message, IMessageHandlerContext context)
        {
            base.Data.CommentUri = message.CommentUri;

            return context.Send(new RequestCheckPullRequestStatus(base.Data.CommentUri, base.Data.ETag));
        }

        public Task Handle(ResponseCheckPullRequestStatus message, IMessageHandlerContext context)
        {
            var answerStatus = message.PullRequestStatus switch
            {
                PullRequestStatus.Open => AnswerStatus.NotAdded,
                PullRequestStatus.Merged => AnswerStatus.Approved,
                PullRequestStatus.Closed => AnswerStatus.Rejected,
                _ => throw new ArgumentOutOfRangeException($"Not supported pull request status: {message.PullRequestStatus}")
            };

            switch (answerStatus)
            {
                case AnswerStatus.NotAdded:
                    base.Data.ETag = message.ETag;

                    return this.RequestTimeout<TimeoutCheckCommentAnswer>(
                        context,
                        TimeSpan.FromSeconds(this.logic.CheckCommentAnswerTimeoutInSeconds));

                case AnswerStatus.Approved:
                    this.MarkAsComplete();
                    return context.Publish(new CommentApproved(base.Data.CommentId));

                case AnswerStatus.Rejected:
                    this.MarkAsComplete();
                    return context.Publish(new CommentRejected(base.Data.CommentId));

                default:
                    throw new ArgumentOutOfRangeException($"Not supported comment answer status: {answerStatus}");
            }
        }
        
        public Task Timeout(TimeoutCheckCommentAnswer state, IMessageHandlerContext context)
        {
            return context.Send(new RequestCheckPullRequestStatus(this.Data.CommentUri, this.Data.ETag));
        }        

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PolicyData> mapper)
        {
            mapper.ConfigureMapping<CheckCommentAnswer>(message => message.CommentId)
                  .ToSaga(data => data.CommentId);
        }
        
        public class PolicyData : ContainSagaData
        {
            public Guid CommentId { get; set; }

            public string CommentUri { get; set; }

            public string ETag { get; set; }
        }        
    }
}