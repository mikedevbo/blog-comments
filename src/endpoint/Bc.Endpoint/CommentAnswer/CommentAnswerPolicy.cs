using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint;
using Bc.Contracts.Internals.Endpoint.CommentAnswer;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.CommentAnswer
{
    public class CommentAnswerPolicy :
        Saga<CommentAnswerPolicy.CommentAnswerPolicyData>,
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
                case CommentAnswerStatus.Rejected:
                    this.MarkAsComplete();
                    break;

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

                default:
                    throw new ArgumentException($"Not supported comment answer status: {message.AnswerStatus}");
            }
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CommentAnswerPolicyData> mapper)
        {
            mapper.ConfigureMapping<CheckCommentAnswerCmd>(message => message.CommentId).ToSaga(data => data.CommentId);
        }
        
        public class CommentAnswerPolicyData : ContainSagaData
        {
            public Guid CommentId { get; set; }

            public string CommentUri { get; set; }

            public string ETag { get; set; }
        }
    }
}