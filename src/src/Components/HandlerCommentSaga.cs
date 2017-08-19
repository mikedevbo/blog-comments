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

        public Task Handle(StartAddingComment message, IMessageHandlerContext context)
        {
            this.Data.CommentId = message.CommentId;
            this.Data.UserName = message.UserName;
            this.Data.UserEmail = message.UserEmail;
            this.Data.UserWebsite = message.UserWebsite;
            this.Data.FileName = message.FileName;
            this.Data.Content = message.Content;

            return context.Send<CreateBranch>(command => command.CommentId = this.Data.CommentId);
        }

        public Task Handle(IBranchCreated message, IMessageHandlerContext context)
        {
            this.Data.BranchName = message.CreatedBranchName;

            return context.Send<AddComment>(command =>
             {
                 command.CommentId = this.Data.CommentId;
                 command.UserName = this.Data.UserName;
                 command.BranchName = this.Data.BranchName;
                 command.FileName = this.Data.FileName;
                 command.Content = this.Data.Content;
             });
        }

        public Task Handle(ICommentAdded message, IMessageHandlerContext context)
        {
            return context.Send<CreatePullRequest>(command =>
             {
                 command.CommentId = this.Data.CommentId;
                 command.CommentBranchName = this.Data.BranchName;
                 command.BaseBranchName = this.componentsConfigurationManager.MasterBranchName;
             });
        }

        public Task Handle(IPullRequestCreated message, IMessageHandlerContext context)
        {
            this.Data.PullRequestLocation = message.PullRequestLocation;

            return this.RequestTimeout(
                context,
                TimeSpan.FromSeconds(this.componentsConfigurationManager.CommentResponseAddedSagaTimeoutInSeconds),
                new CheckCommentResponseTimeout { CommentId = this.Data.CommentId });
        }

        public Task Timeout(CheckCommentResponseTimeout state, IMessageHandlerContext context)
        {
            return context.Send<CheckCommentResponse>(command => command.CommentId = this.Data.CommentId);
        }

        public async Task Handle(ICommentResponseAdded message, IMessageHandlerContext context)
        {
            if (message.CommentResponseStatus == CommentResponseStatus.Approved)
            {
                await context.Send<SendEmail>(command => command.EmailAddress = this.Data.UserEmail)
                    .ConfigureAwait(false);

                this.MarkAsComplete();
            }
            else
            {
                await this.RequestTimeout(
                    context,
                    TimeSpan.FromSeconds(this.componentsConfigurationManager.CommentResponseAddedSagaTimeoutInSeconds),
                    new CheckCommentResponseTimeout { CommentId = this.Data.CommentId })
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
