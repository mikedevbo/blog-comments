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
            sb.Append(DateTime.UtcNow).Append(Guid.NewGuid());
            string branchName = sb.ToString();

            ////TODO: Is this idempotent ?
            ////TODO: Is this can be awaitable ?
            this.gitHubApi.CreateRepositoryBranch(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                this.configurationManager.RepositoryName,
                this.configurationManager.MasterBranchName,
                branchName);

            await context.Publish<IBranchCreated>(evt => evt.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }
    }
}
