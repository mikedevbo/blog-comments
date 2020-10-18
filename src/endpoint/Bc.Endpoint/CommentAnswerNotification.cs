using System;
using System.Threading.Tasks;
using Bc.Contracts.Externals.Endpoint.CommentAnswer.Events;
using Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands;
using Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Logic;
using NServiceBus;
using NServiceBus.Mailer;

namespace Bc.Endpoint
{
    public class CommentAnswerNotificationEventSubscribingPolicy :
        IHandleMessages<CommentApproved>,
        IHandleMessages<CommentRejected>
    {
        public Task Handle(CommentApproved message, IMessageHandlerContext context)
        {
            return context.Send(new NotifyAboutCommentAnswer(message.CommentId, true));
        }

        public Task Handle(CommentRejected message, IMessageHandlerContext context)
        {
            return context.Send(new NotifyAboutCommentAnswer(message.CommentId, false));
        }
    }

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
            this.Data.IsNotificationRegistered = true;

            return this.SendNotification(context);
        }

        public Task Handle(NotifyAboutCommentAnswer message, IMessageHandlerContext context)
        {
            this.Data.IsCommentApproved = message.IsApproved;
            this.Data.IsNotificationReadyToSend = true;

            return this.SendNotification(context);
        }

        private Task SendNotification(IMessageHandlerContext context)
        {
            if (!this.Data.IsNotificationRegistered || !this.Data.IsNotificationReadyToSend)
            {
                return Task.CompletedTask;
            }

            this.MarkAsComplete();

            if (!this.logic.IsSendNotification(this.Data.IsCommentApproved, this.Data.UserEmail))
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

            public bool IsCommentApproved { get; set; }

            public bool IsNotificationRegistered { get; set; }

            public bool IsNotificationReadyToSend { get; set; }
        }
    }
}