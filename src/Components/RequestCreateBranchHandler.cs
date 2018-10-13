namespace Components
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Messages.Messages;
    using NServiceBus;

    public class RequestCreateBranchHandler : IHandleMessages<RequestCreateBranch>
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IGitHubApi gitHubApi;

        public RequestCreateBranchHandler(IConfigurationManager configurationManager, IGitHubApi gitHubApi)
        {
            this.configurationManager = configurationManager;
            this.gitHubApi = gitHubApi;
        }

        public async Task Handle(RequestCreateBranch message, IMessageHandlerContext context)
        {
            var sb = new StringBuilder();
            sb.Append("c-").Append(message.AddedDate.ToString("yyyy-MM-dd-HH-mm-ss-fff"));
            string branchName = sb.ToString();

            await this.gitHubApi.CreateRepositoryBranch(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                this.configurationManager.RepositoryName,
                this.configurationManager.MasterBranchName,
                branchName).ConfigureAwait(false);

            await context.Reply<CreateBranchResponse>(
                response =>
                {
                    response.CreatedBranchName = branchName;
                })
                .ConfigureAwait(false);
        }
    }
}
