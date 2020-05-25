using System;
using System.Threading.Tasks;
using Bc.Contracts.Externals.Endpoint.CommentAnswer.Events;
using Bc.Contracts.Externals.Endpoint.CommentRegistration.Events;
using Bc.Contracts.Internals.Endpoint;
using Bc.Contracts.Internals.Endpoint.CommentAnswer;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Messages;
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
        IHandleMessages<ResponseCheckCommentAnswer>
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
            return context.Send(new RequestCheckCommentAnswer(this.Data.CommentUri, this.Data.ETag));
        }

        public async Task Handle(ResponseCheckCommentAnswer message, IMessageHandlerContext context)
        {
            switch (message.AnswerStatus)
            {
                case CommentAnswerStatus.NotAdded:
                    this.Data.ETag = message.ETag;

                    await this.RequestTimeout<TimeoutCheckCommentAnswer>(
                        context,
                        TimeSpan.FromSeconds(this.configurationProvider.CheckCommentAnswerTimeoutInSeconds));
                    break;

                case CommentAnswerStatus.Approved:
                    await context.Publish(new CommentApproved(this.Data.CommentId)).ConfigureAwait(false);
                    this.MarkAsComplete();
                    break;

                case CommentAnswerStatus.Rejected:
                    await context.Publish(new CommentRejected(this.Data.CommentId)).ConfigureAwait(false);
                    this.MarkAsComplete();
                    break;

                default:
                    throw new ArgumentException($"Not supported comment answer status: {message.AnswerStatus}");
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

    public class CommentAnswerPolicyHandlers :
        IHandleMessages<CommentRegistered>,
        IHandleMessages<RequestCheckCommentAnswer>
    {
        private static readonly ILog Log = LogManager.GetLogger<CommentAnswerPolicyHandlers>();
        private readonly ICommentAnswerPolicyLogic logic;

        public CommentAnswerPolicyHandlers(ICommentAnswerPolicyLogic logic)
        {
            this.logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }
        
        public Task Handle(CommentRegistered message, IMessageHandlerContext context)
        {
            Log.Info($"{this.GetType().Name} {message.CommentId}");
            return context.Send(new CheckCommentAnswer(message.CommentId, message.CommentUri));
        }

        public async Task Handle(RequestCheckCommentAnswer message, IMessageHandlerContext context)
        {
            var response = await this.logic.CheckAnswer(message.CommentUri, message.Etag).ConfigureAwait(false);
            await context.Reply(response).ConfigureAwait(false);
        }
    }
}