using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswer;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.CommentAnswer
{
    public class CommentAnswerPolicy :
        Saga<CommentAnswerPolicy.CommentAnswerPolicyData>,
        IAmStartedByMessages<CheckCommentAnswerCmd>
    {
        private static readonly ILog Log = LogManager.GetLogger<CommentAnswerPolicy>();

        public Task Handle(CheckCommentAnswerCmd message, IMessageHandlerContext context)
        {
            this.Data.CommentUri = message.CommentUri;
            
            ////TODO: Add logic
            Log.Info($"{this.GetType().Name} {message.CommentId}");
            
            this.MarkAsComplete();
            return context.Publish(new CommentAnswerAddedEvt(this.Data.CommentId, true));
        }        
        
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CommentAnswerPolicyData> mapper)
        {
            mapper.ConfigureMapping<CheckCommentAnswerCmd>(message => message.CommentId).ToSaga(data => data.CommentId);
        }
        
        public class CommentAnswerPolicyData : ContainSagaData
        {
            public Guid CommentId { get; set; }

            public string CommentUri { get; set; }
        }
    }
}