using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands;
using NServiceBus;

namespace Bc.Endpoint
{
    public class CommentAnswerNotificationPolicy :
        Saga<CommentAnswerNotificationPolicy.PolicyData>,
        IAmStartedByMessages<RegisterCommentNotification>,
        IAmStartedByMessages<NotifyAboutCommentAnswer>
    {
        public Task Handle(RegisterCommentNotification message, IMessageHandlerContext context)
        {
            this.Data.UserEmail = message.UserEmail;
            return Task.CompletedTask;
        }

        public Task Handle(NotifyAboutCommentAnswer message, IMessageHandlerContext context)
        {
            ////TODO: Add logic
            this.MarkAsComplete();
            return Task.CompletedTask;
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PolicyData> mapper)
        {
            mapper.ConfigureMapping<RegisterCommentNotification>(message => message.CommentId)
                  .ToSaga(data => data.CommentId);
            
            mapper.ConfigureMapping<NotifyAboutCommentAnswer>(message => message.CommentId)
                  .ToSaga(data => data.CommentId);
        }
        
        public class PolicyData : ContainSagaData
        {
            public Guid CommentId { get; set; }

            public string UserEmail { get; set; }
        }        
    }
}