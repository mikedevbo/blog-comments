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
        IHandleMessages<IBranchCreated>,
        IHandleMessages<ICommentAdded>,
        IHandleMessages<IPullRequestCreated>,
        IHandleTimeouts<CheckCommentResponseTimeout>,
        IHandleMessages<ICommentResponseAdded>
    {
        private ILog log = LogManager.GetLogger<HandlerCommentSaga>();
        private readonly int timeoutMinutes = 30;

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CommentSagaData> mapper)
        {
            mapper.ConfigureMapping<StartAddingComment>(message => message.CommentId)
                .ToSaga(sagaData => sagaData.CommentId);

            mapper.ConfigureMapping<IBranchCreated>(message => message.CommentId)
                            .ToSaga(sagaData => sagaData.CommentId);

            mapper.ConfigureMapping<ICommentAdded>(message => message.CommentId)
                .ToSaga(sagaData => sagaData.CommentId);

            mapper.ConfigureMapping<IPullRequestCreated>(message => message.CommentId)
                .ToSaga(sagaData => sagaData.CommentId);

            mapper.ConfigureMapping<CheckCommentResponseTimeout>(message => message.CommentId)
                .ToSaga(sagaData => sagaData.CommentId);

            mapper.ConfigureMapping<ICommentResponseAdded>(message => message.CommentId)
                .ToSaga(sagaData => sagaData.CommentId);
        }

        public async Task Handle(StartAddingComment message, IMessageHandlerContext context)
        {
            await context.Send<CreateBranch>(command => command.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }

        public async Task Handle(IBranchCreated message, IMessageHandlerContext context)
        {
            await context.Send<AddComment>(command => command.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }

        public async Task Handle(ICommentAdded message, IMessageHandlerContext context)
        {
            await context.Send<CreatePullRequest>(command => command.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }

        public async Task Handle(IPullRequestCreated message, IMessageHandlerContext context)
        {
            await RequestTimeout<CheckCommentResponseTimeout>(context, TimeSpan.FromMinutes(this.timeoutMinutes))
                .ConfigureAwait(false);
        }

        public async Task Timeout(CheckCommentResponseTimeout state, IMessageHandlerContext context)
        {
            await context.Send<CheckCommentResponse>(command => command.CommentId = state.CommentId)
                .ConfigureAwait(false);
        }

        public async Task Handle(ICommentResponseAdded message, IMessageHandlerContext context)
        {
            if (message.CommentResponseState == CommentResponseState.Added)
            {
                await context.Send<SendEmail>(command => command.EmailAddress = this.Data.UserEmailAddress)
                    .ConfigureAwait(false);

                MarkAsComplete();
            }
            else
            {
                await RequestTimeout<CheckCommentResponseTimeout>(context, TimeSpan.FromMinutes(this.timeoutMinutes));
            }
        }
    }
}
