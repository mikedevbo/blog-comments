namespace Components
{
    using System;
    using System.Threading.Tasks;
    using Messages;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus;
    using NServiceBus.Logging;
    using NServiceBus.Persistence.Sql;

    public class HandlerCommentSaga :
        SqlSaga<CommentSagaData>,
        IAmStartedByMessages<StartAddingComment>,
        IHandleMessages<IBranchCreated>,
        IHandleMessages<ICommentAdded>,
        IHandleMessages<IPullRequestCreated>,
        IHandleTimeouts<CheckCommentResponseTimeout>,
        IHandleMessages<ICommentResponseAdded>
    {
        private readonly IComponentsConfigurationManager componentsConfigurationManager;
        private ILog log = LogManager.GetLogger<HandlerCommentSaga>();

        public HandlerCommentSaga()
        {
            ////for unit tests only
        }

        public HandlerCommentSaga(IComponentsConfigurationManager componentsConfigurationManager)
        {
            this.componentsConfigurationManager = componentsConfigurationManager;
        }

        protected override string CorrelationPropertyName => nameof(CommentSagaData.CommentId);

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
            await this.RequestTimeout(
                context,
                TimeSpan.FromSeconds(this.componentsConfigurationManager.CommentResponseAddedSagaTimeoutInSeconds),
                new CheckCommentResponseTimeout { CommentId = message.CommentId })
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

                this.MarkAsComplete();
            }
            else
            {
                await this.RequestTimeout(
                    context,
                    TimeSpan.FromSeconds(this.componentsConfigurationManager.CommentResponseAddedSagaTimeoutInSeconds),
                    new CheckCommentResponseTimeout { CommentId = message.CommentId })
                    .ConfigureAwait(false);
            }
        }

        protected override void ConfigureMapping(IMessagePropertyMapper mapper)
        {
            mapper.ConfigureMapping<StartAddingComment>(message => message.CommentId);
            mapper.ConfigureMapping<IBranchCreated>(message => message.CommentId);
            mapper.ConfigureMapping<ICommentAdded>(message => message.CommentId);
            mapper.ConfigureMapping<IPullRequestCreated>(message => message.CommentId);
            mapper.ConfigureMapping<CheckCommentResponseTimeout>(message => message.CommentId);
            mapper.ConfigureMapping<ICommentResponseAdded>(message => message.CommentId);
        }
    }
}
