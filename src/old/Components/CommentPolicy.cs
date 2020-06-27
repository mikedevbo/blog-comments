namespace Components
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Messages;
    using Messages.Commands;
    using Messages.Messages;
    using NServiceBus;
    using NServiceBus.Logging;

    public class CommentPolicy :
        Saga<CommentPolicy.CommentPolicyData>,
        IAmStartedByMessages<StartAddingComment>,
        IHandleMessages<CreateBranchResponse>,
        IHandleMessages<AddCommentResponse>,
        IHandleMessages<CreatePullRequestResponse>,
        IHandleTimeouts<CheckCommentAnswerTimeout>,
        IHandleMessages<CheckCommentAnswerResponse>
    {
        private readonly IConfigurationManager configurationManager;
        private readonly ILog log = LogManager.GetLogger<CommentPolicy>();

        public CommentPolicy()
        {
            ////for unit tests only
        }

        public CommentPolicy(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public Task Handle(StartAddingComment message, IMessageHandlerContext context)
        {
            this.Data.CommentId = message.CommentId;
            this.Data.UserName = message.UserName;
            this.Data.UserEmail = message.UserEmail;
            this.Data.UserWebsite = message.UserWebsite;
            this.Data.FileName = message.FileName;
            this.Data.Content = message.Content;
            this.Data.AddedDate = message.AddedDate;

            return context.Send(new RequestCreateBranch(this.Data.AddedDate));
        }

        public Task Handle(CreateBranchResponse message, IMessageHandlerContext context)
        {
            this.Data.BranchName = message.CreatedBranchName;

            return context.Send<RequestAddComment>(msg =>
            {
                msg.UserName = this.Data.UserName;
                msg.BranchName = this.Data.BranchName;
                msg.FileName = this.Data.FileName;
                msg.Content = this.Data.Content;
                msg.AddedDate = this.Data.AddedDate;
                msg.UserWebSite = this.Data.UserWebsite;
            });
        }

        public Task Handle(AddCommentResponse message, IMessageHandlerContext context)
        {
            return context.Send<RequestCreatePullRequest>(msg =>
            {
                msg.CommentBranchName = this.Data.BranchName;
                msg.BaseBranchName = this.configurationManager.MasterBranchName;
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
            return context.Send<RequestCheckCommentAnswer>(msg =>
            {
                msg.PullRequestUri = this.Data.PullRequestLocation;
                msg.Etag = this.Data.ETag;
            });
        }

        public async Task Handle(CheckCommentAnswerResponse message, IMessageHandlerContext context)
        {
            switch (message.Status)
            {
                case CommentAnswerStatus.Rejected:
                    this.MarkAsComplete();
                    break;

                case CommentAnswerStatus.NotAddded:
                    this.Data.ETag = message.ETag;

                    await this.RequestTimeout<CheckCommentAnswerTimeout>(
                        context,
                        TimeSpan.FromSeconds(this.configurationManager.CommentResponseAddedSagaTimeoutInSeconds)).ConfigureAwait(false);
                    break;

                case CommentAnswerStatus.Approved:
                    await context.Send<SendEmail>(msg =>
                    {
                        msg.UserName = this.Data.UserName;
                        msg.UserEmail = this.Data.UserEmail;
                        msg.FileName = this.Data.FileName;
                        msg.CommentResponseStatus = message.Status;
                    }).ConfigureAwait(false);

                    this.MarkAsComplete();
                    break;

                default:
                    throw new ArgumentException($"Not supported comment response status: {message.Status}");
            }
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CommentPolicyData> mapper)
        {
            mapper.ConfigureMapping<StartAddingComment>(message => message.CommentId).ToSaga(sagaData => sagaData.CommentId);
        }

        public class CommentPolicyData : ContainSagaData
        {
            public Guid CommentId { get; set; }

            public string UserName { get; set; }

            public string UserEmail { get; set; }

            public string UserWebsite { get; set; }

            public string FileName { get; set; }

            public string Content { get; set; }

            public string BranchName { get; set; }

            public string PullRequestLocation { get; set; }

            public string ETag { get; set; }

            public DateTime AddedDate { get; set; }
        }
    }
}
