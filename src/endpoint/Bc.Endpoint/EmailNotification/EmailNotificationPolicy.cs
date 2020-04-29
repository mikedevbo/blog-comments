using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Events;
using Bc.Contracts.Internals.Endpoint.EmailNotification.Commands;
using NServiceBus;

namespace Bc.Endpoint.EmailNotification
{
    public class EmailNotificationPolicy :
        Saga<EmailNotificationPolicy.SendEmailNotificationPolicyData>,
        IAmStartedByMessages<SendEmailNotification>,
        IAmStartedByMessages<CommentAnswerAdded>
    {
        public Task Handle(SendEmailNotification message, IMessageHandlerContext context)
        {
            this.Data.UserEmail = message.UserEmail;
            
            ////TODO: Add logic
            return Task.CompletedTask;
        }

        public Task Handle(CommentAnswerAdded message, IMessageHandlerContext context)
        {
            ////TODO: Add logic
            return Task.CompletedTask;
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SendEmailNotificationPolicyData> mapper)
        {
            mapper.ConfigureMapping<SendEmailNotification>(message => message.CommentId).ToSaga(data => data.CommentId);
            mapper.ConfigureMapping<CommentAnswerAdded>(message => message.CommentId).ToSaga(data => data.CommentId);
        }
        
        public class SendEmailNotificationPolicyData : ContainSagaData
        {
            public Guid CommentId { get; set; }

            public string UserEmail { get; set; }
        }        
    }
}