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

    /// <summary>
    /// The saga.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerCommentSaga"/> class.
        /// </summary>
        public HandlerCommentSaga()
        {
            ////for unit tests only
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerCommentSaga"/> class.
        /// </summary>
        /// <param name="componentsConfigurationManager">The components configuration manager.</param>
        public HandlerCommentSaga(IComponentsConfigurationManager componentsConfigurationManager)
        {
            this.componentsConfigurationManager = componentsConfigurationManager;
        }

        /// <summary>
        /// Gets the name of the correlation property.
        /// </summary>
        protected override string CorrelationPropertyName => nameof(CommentSagaData.CommentId);

        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns>Represents the async method.</returns>
        public async Task Handle(StartAddingComment message, IMessageHandlerContext context)
        {
            await context.Send<CreateBranch>(command => command.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Handles a message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="context">The context of the currently handled message.</param>
        /// <returns>Represents the async method.</returns>
        /// <remarks>
        /// This method will be called when a message arrives on at the endpoint and should contain
        /// the custom logic to execute when the message is received.
        /// </remarks>
        public async Task Handle(IBranchCreated message, IMessageHandlerContext context)
        {
            await context.Send<AddComment>(command => command.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Handles a message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="context">The context of the currently handled message.</param>
        /// <returns>Represents the async method.</returns>
        /// <remarks>
        /// This method will be called when a message arrives on at the endpoint and should contain
        /// the custom logic to execute when the message is received.
        /// </remarks>
        public async Task Handle(ICommentAdded message, IMessageHandlerContext context)
        {
            await context.Send<CreatePullRequest>(command => command.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Handles a message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="context">The context of the currently handled message.</param>
        /// <returns>Represents the async method.</returns>
        /// <remarks>
        /// This method will be called when a message arrives on at the endpoint and should contain
        /// the custom logic to execute when the message is received.
        /// </remarks>
        public async Task Handle(IPullRequestCreated message, IMessageHandlerContext context)
        {
            await this.RequestTimeout(
                context,
                TimeSpan.FromSeconds(this.componentsConfigurationManager.CommentResponseAddedSagaTimeoutInSeconds),
                new CheckCommentResponseTimeout { CommentId = message.CommentId })
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Called when the timeout has expired.
        /// </summary>
        /// <param name="state">The timeout state.</param>
        /// <param name="context">The context.</param>
        /// <returns>Represents the async method.</returns>
        public async Task Timeout(CheckCommentResponseTimeout state, IMessageHandlerContext context)
        {
            await context.Send<CheckCommentResponse>(command => command.CommentId = state.CommentId)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Handles a message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="context">The context of the currently handled message.</param>
        /// <returns>Represents the async method.</returns>
        /// <remarks>
        /// This method will be called when a message arrives on at the endpoint and should contain
        /// the custom logic to execute when the message is received.
        /// </remarks>
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

        /// <summary>
        /// Allows messages to be mapped to <see cref="P:NServiceBus.Persistence.Sql.SqlSaga`1.CorrelationPropertyName" />.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
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
