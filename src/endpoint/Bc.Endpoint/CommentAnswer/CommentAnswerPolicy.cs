using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint;
using Bc.Contracts.Internals.Endpoint.CommentAnswer;
using Bc.Contracts.Internals.Endpoint.CommentRegistration;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.CommentAnswer
{
    public class CommentAnswerPolicy :
        Saga<CommentAnswerPolicyData>,
        IAmStartedByMessages<CheckCommentAnswerCmd>,
        IHandleTimeouts<CheckCommentAnswerTimeoutMsg>,
        IHandleMessages<CheckCommentAnswerMsgResponseMsg>
    {
        private static readonly ILog Log = LogManager.GetLogger<CommentAnswerPolicy>();
        private readonly IEndpointConfigurationProvider configurationProvider;

        public CommentAnswerPolicy(IEndpointConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        }

        public Task Handle(CheckCommentAnswerCmd message, IMessageHandlerContext context)
        {
            this.Data.CommentUri = message.CommentUri;

            return this.RequestTimeout<CheckCommentAnswerTimeoutMsg>(
                context,
                TimeSpan.FromSeconds(this.configurationProvider.CheckCommentAnswerTimeoutInSeconds));
        }

        public Task Timeout(CheckCommentAnswerTimeoutMsg state, IMessageHandlerContext context)
        {
            return context.Send(new RequestCheckCommentAnswerMsg(this.Data.CommentUri, this.Data.ETag));
        }

        public async Task Handle(CheckCommentAnswerMsgResponseMsg message, IMessageHandlerContext context)
        {
            switch (message.AnswerStatus)
            {
                case CommentAnswerStatus.NotAdded:
                    this.Data.ETag = message.ETag;

                    await this.RequestTimeout<CheckCommentAnswerTimeoutMsg>(
                        context,
                        TimeSpan.FromSeconds(this.configurationProvider.CheckCommentAnswerTimeoutInSeconds));
                    break;

                case CommentAnswerStatus.Approved:
                    await context.Publish(new CommentApprovedEvt(this.Data.CommentId)).ConfigureAwait(false);
                    this.MarkAsComplete();
                    break;

                case CommentAnswerStatus.Rejected:
                    await context.Publish(new CommentRejectedEvt(this.Data.CommentId)).ConfigureAwait(false);
                    this.MarkAsComplete();
                    break;

                default:
                    throw new ArgumentException($"Not supported comment answer status: {message.AnswerStatus}");
            }
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CommentAnswerPolicyData> mapper)
        {
            mapper.ConfigureMapping<CheckCommentAnswerCmd>(message => message.CommentId).ToSaga(data => data.CommentId);
        }
    }

    public class CommentAnswerPolicyData : ContainSagaData
    {
        public Guid CommentId { get; set; }

        public string CommentUri { get; set; }

        public string ETag { get; set; }
    }

    public class CommentAnswerPolicyHandlers :
        IHandleMessages<CommentRegisteredEvt>,
        IHandleMessages<RequestCheckCommentAnswerMsg>
    {
        private static readonly ILog Log = LogManager.GetLogger<CommentAnswerPolicyHandlers>();
        private readonly ICommentAnswerPolicyLogic logic;

        public CommentAnswerPolicyHandlers(ICommentAnswerPolicyLogic logic)
        {
            this.logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }
        
        public Task Handle(CommentRegisteredEvt message, IMessageHandlerContext context)
        {
            Log.Info($"{this.GetType().Name} {message.CommentId}");
            return context.Send(new CheckCommentAnswerCmd(message.CommentId, message.CommentUri));
        }

        public async Task Handle(RequestCheckCommentAnswerMsg message, IMessageHandlerContext context)
        {
            var response = await this.logic.CheckAnswer(message.CommentUri, message.Etag).ConfigureAwait(false);
            await context.Reply(response).ConfigureAwait(false);
        }
    }
}