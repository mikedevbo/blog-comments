using Components.GitHub;
using Components.GitHub.Dto;
using Messages.Commands;
using Messages.Events;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class HandlerCreateGitHubBranch : IHandleMessages<CreateGitHubBranch>
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IGitHubApi gitHubApi;

        public HandlerCreateGitHubBranch(IConfigurationManager configurationManager, IGitHubApi gitHubApi)
        {
            this.configurationManager = configurationManager;
            this.gitHubApi = gitHubApi;
        }

        public async Task Handle(CreateGitHubBranch message, IMessageHandlerContext context)
        {
            string userAgent = this.configurationManager.UserAgent;
            string authorizationToken = this.configurationManager.AuthorizationToken;
            string masterRepositoryName = this.configurationManager.MasterRepositoryName;

            Repository masterRepo = this.gitHubApi.GetRepository(
                userAgent,
                authorizationToken,
                masterRepositoryName);

            string sha = masterRepo.Object.Sha;

            var sb = new StringBuilder();
            sb.Append(DateTime.UtcNow).Append(Guid.NewGuid());
            string branchName = sb.ToString();

            ////Is this idempotent ?
            this.gitHubApi.CreateRepositoryBranch(
                userAgent,
                authorizationToken,
                masterRepositoryName,
                sha,
                branchName);

            await context.Publish<IGitHubBranchCreated>(evt => evt.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }
    }
}
