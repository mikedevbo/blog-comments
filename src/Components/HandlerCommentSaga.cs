namespace Components
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Messages;
    using Messages.Commands;
    using Messages.Events;
    using Messages.Messages;
    using NServiceBus;
    using NServiceBus.Logging;

    public class HandlerCommentSaga :
       Saga<CommentSagaData>,
        IAmStartedByMessages<StartAddingComment>,
        IHandleMessages<ICommentResponseAdded>,
        IHandleMessages<CreateBranchResponse>,
        IHandleMessages<AddCommentResponse>,
        IHandleMessages<CreatePullRequestResponse>,
        IHandleTimeouts<CheckCommentAnswerTimeout>
    {
        private readonly IConfigurationManager configurationManager;
        private readonly ILog log = LogManager.GetLogger<HandlerCommentSaga>();

        public HandlerCommentSaga()
        {
            ////for unit tests only
        }

        public HandlerCommentSaga(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public Task Handle(StartAddingComment message, IMessageHandlerContext context)
        {
            this.Data.UserName = message.UserName;
            this.Data.UserEmail = message.UserEmail;
            this.Data.UserWebsite = message.UserWebsite;
            this.Data.FileName = message.FileName;
            this.Data.Content = message.Content;

            return context.Send(new RequestCreateBranch());
        }

        public Task Handle(CreateBranchResponse message, IMessageHandlerContext context)
        {
            this.Data.BranchName = message.CreatedBranchName;

            return context.Send<RequestAddComment>(command =>
            {
                command.UserName = this.Data.UserName;
                command.BranchName = this.Data.BranchName;
                command.FileName = this.Data.FileName;
                command.Content = this.Data.Content;
            });
        }

        public Task Handle(AddCommentResponse message, IMessageHandlerContext context)
        {
            return context.Send<RequestCreatePullRequest>(command =>
            {
                command.CommentBranchName = this.Data.BranchName;
                command.BaseBranchName = this.configurationManager.MasterBranchName;
            });
        }

        public Task Handle(CreatePullRequestResponse message, IMessageHandlerContext context)
        {
            this.Data.PullRequestLocation = message.PullRequestLocation;

            return this.RequestTimeout<CheckCommentAnswerTimeout>(
                 context,
                 TimeSpan.FromSeconds(this.configurationManager.CommentResponseAddedSagaTimeoutInSeconds));
        }

        public Task Timeout(CheckCommentAnswerTimeout state, IMessageHandlerContext context)
        {
            return context.Send<CheckCommentResponse>(command =>
            {
                command.CommentId = this.Data.CommentId;
                command.PullRequestUri = this.Data.PullRequestLocation;
                command.Etag = this.Data.ETag;
            });
        }

        public async Task Handle(ICommentResponseAdded message, IMessageHandlerContext context)
        {
            if (message.CommentResponse.ResponseStatus == CommentResponseStatus.Approved ||
                message.CommentResponse.ResponseStatus == CommentResponseStatus.Rejected)
            {
                await context.Send<SendEmail>(command =>
                {
                    command.UserName = this.Data.UserName;
                    command.UserEmail = this.Data.UserEmail;
                    command.FileName = this.Data.FileName;
                    command.CommentResponseStatus = message.CommentResponse.ResponseStatus;
                }).ConfigureAwait(false);

                this.MarkAsComplete();
            }
            else
            {
                this.Data.ETag = message.CommentResponse.ETag;

                await this.RequestTimeout<CheckCommentAnswerTimeout>(
                    context,
                    TimeSpan.FromSeconds(this.configurationManager.CommentResponseAddedSagaTimeoutInSeconds)).ConfigureAwait(false);
            }
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CommentSagaData> mapper)
        {
            mapper.ConfigureMapping<StartAddingComment>(message => message.CommentId).ToSaga(sagaData => sagaData.CommentId);
            mapper.ConfigureMapping<ICommentResponseAdded>(message => message.CommentId).ToSaga(sagaData => sagaData.CommentId);
        }
    }
}
