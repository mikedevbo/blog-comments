using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.EmailNotification;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.EmailNotification
{
    public class EmailNotificationPolicy :
        Saga<EmailNotificationPolicy.SendEmailNotificationPolicyData>,
        IAmStartedByMessages<NotifyByEmailCmd>,
        IAmStartedByMessages<NotifyAnswerByEmailCmd>
    {
        private static readonly ILog Log = LogManager.GetLogger<EmailNotificationPolicy>();

        public Task Handle(NotifyByEmailCmd message, IMessageHandlerContext context)
        {
            this.Data.UserEmail = message.UserEmail;
            return Task.CompletedTask;
        }

        public Task Handle(NotifyAnswerByEmailCmd message, IMessageHandlerContext context)
        {
            ////TODO: Add logic
            Log.Info($"{this.GetType().Name} {message.CommentId}");
            this.MarkAsComplete();
            return Task.CompletedTask;
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SendEmailNotificationPolicyData> mapper)
        {
            mapper.ConfigureMapping<NotifyByEmailCmd>(message => message.CommentId).ToSaga(data => data.CommentId);
            mapper.ConfigureMapping<NotifyAnswerByEmailCmd>(message => message.CommentId).ToSaga(data => data.CommentId);
        }
        
        public class SendEmailNotificationPolicyData : ContainSagaData
        {
            public Guid CommentId { get; set; }

            public string UserEmail { get; set; }
        }        
    }
}