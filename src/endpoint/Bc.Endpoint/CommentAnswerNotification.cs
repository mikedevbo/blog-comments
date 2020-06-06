using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands;
using Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Logic;
using NServiceBus;
using NServiceBus.Mailer;

namespace Bc.Endpoint
{
    public class CommentAnswerNotificationPolicy :
        Saga<CommentAnswerNotificationPolicy.PolicyData>,
        IAmStartedByMessages<RegisterCommentNotification>,
        IAmStartedByMessages<NotifyAboutCommentAnswer>
    {
        private readonly ICommentAnswerNotificationPolicyLogic logic;

        public CommentAnswerNotificationPolicy(ICommentAnswerNotificationPolicyLogic logic)
        {
            this.logic = logic;
        }

        public Task Handle(RegisterCommentNotification message, IMessageHandlerContext context)
        {
            this.Data.UserEmail = message.UserEmail;
            this.Data.ArticleFileName = message.ArticleFileName;
            return Task.CompletedTask;
        }

        public Task Handle(NotifyAboutCommentAnswer message, IMessageHandlerContext context)
        {
            if (!this.logic.IsSendNotification(message, this.Data.UserEmail))
            {
                return Task.CompletedTask;
            }

            var mail = new Mail
            {
                From = this.logic.From,
                To = this.Data.UserEmail,
                Subject = this.logic.Subject,
                Body = this.logic.GetBody(this.Data.ArticleFileName)
            };

            this.MarkAsComplete();
            return context.SendMail(mail);
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
            
            public string ArticleFileName { get; set; }
        }        
    }
}