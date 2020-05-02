using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswer;
using NServiceBus;

namespace Bc.Endpoint.CommentAnswer
{
    public class CommentAnswerPolicy :
        Saga<CommentAnswerPolicy.CommentAnswerPolicyData>,
        IAmStartedByMessages<CheckCommentAnswerCmd>
    {
        public Task Handle(CheckCommentAnswerCmd message, IMessageHandlerContext context)
        {
            this.Data.CommentUri = message.CommentUri;
            
            ////TODO: Add logic
            
            return Task.CompletedTask;
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