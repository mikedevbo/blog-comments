using Messages;
using Messages.Commands;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class HandlerCommentSaga :
        Saga<CommentSagaData>,
        IAmStartedByMessages<StartAddingComment>,
        IHandleMessages<IGitHubBranchCreated>,
        IHandleMessages<ICommentAdded>,
        IHandleMessages<IGitHubPullRequestSent>,
        IHandleTimeouts<CheckCommentResponseTimeout>,
        IHandleMessages<ICommentResponseAdded>
    {
        private ILog log = LogManager.GetLogger<HandlerCommentSaga>();
        private readonly int timeoutMinutes = 30;

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CommentSagaData> mapper)
        {
            mapper.ConfigureMapping<StartAddingComment>(message => message.CommentId)
                .ToSaga(sagaData => sagaData.CommentId);
        }

        public Task Handle(StartAddingComment message, IMessageHandlerContext context)
        {
            context.Send<CreateGitHubBranch>(command => command.CommentId = message.CommentId)
                .ConfigureAwait(false);

            return Task.CompletedTask;
        }

        public Task Handle(IGitHubBranchCreated message, IMessageHandlerContext context)
        {
            context.Send<AddComment>(command => command.CommentId = message.CommentId)
                .ConfigureAwait(false);

            return Task.CompletedTask;
        }

        public Task Handle(ICommentAdded message, IMessageHandlerContext context)
        {
            context.Send<SendGitHubPullRequest>(command => command.CommentId = message.CommentId)
                .ConfigureAwait(false);

            return Task.CompletedTask;
        }

        public Task Handle(IGitHubPullRequestSent message, IMessageHandlerContext context)
        {
            return RequestTimeout<CheckCommentResponseTimeout>(context, TimeSpan.FromMinutes(this.timeoutMinutes));
        }

        public Task Timeout(CheckCommentResponseTimeout state, IMessageHandlerContext context)
        {
            context.Send<CheckCommentResponse>(command => command.CommentId = state.CommentId)
                .ConfigureAwait(false);

            return Task.CompletedTask;
        }

        public Task Handle(ICommentResponseAdded message, IMessageHandlerContext context)
        {
            if (message.CommentResponseState == CommentResponseState.Added)
            {
                context.Send<SendEmail>(command => command.EmailAddress = this.Data.UserEmailAddress)
                    .ConfigureAwait(false);

                MarkAsComplete();

                return Task.CompletedTask;
            }
            else
            {
                return RequestTimeout<CheckCommentResponseTimeout>(context, TimeSpan.FromMinutes(this.timeoutMinutes));
            }
        }
    }
}
