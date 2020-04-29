using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands;
using NServiceBus;

namespace Bc.Endpoint.CommentAnswer
{
    public class CommentAnswerPolicy :
        Saga<CommentAnswerPolicy.CommentAnswerPolicyData>,
        IAmStartedByMessages<CheckCommentAnswer>
    {
        public Task Handle(CheckCommentAnswer message, IMessageHandlerContext context)
        {
            this.Data.CommentUri = message.CommentUri;
            
            ////TODO: Add logic
            
            return Task.CompletedTask;
        }        
        
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CommentAnswerPolicyData> mapper)
        {
            mapper.ConfigureMapping<CheckCommentAnswer>(message => message.CommentId).ToSaga(data => data.CommentId);
        }
        
        public class CommentAnswerPolicyData : ContainSagaData
        {
            public Guid CommentId { get; set; }

            public string CommentUri { get; set; }
        }
    }
}