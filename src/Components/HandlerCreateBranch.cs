namespace Components
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus;

    public class HandlerCreateBranch : IHandleMessages<CreateBranch>
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IGitHubApi gitHubApi;

        public HandlerCreateBranch(IConfigurationManager configurationManager, IGitHubApi gitHubApi)
        {
            this.configurationManager = configurationManager;
            this.gitHubApi = gitHubApi;
        }

        public async Task Handle(CreateBranch message, IMessageHandlerContext context)
        {
            var sb = new StringBuilder();
            sb.Append("c-").Append(DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fff"));
            string branchName = sb.ToString();

            await this.gitHubApi.CreateRepositoryBranch(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                this.configurationManager.RepositoryName,
                this.configurationManager.MasterBranchName,
                branchName).ConfigureAwait(false);

            await context.Publish<IBranchCreated>(
                evt =>
                {
                    evt.CommentId = message.CommentId;
                    evt.CreatedBranchName = branchName;
                })
                .ConfigureAwait(false);
        }
    }
}
