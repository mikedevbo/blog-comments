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
            ////TODO: Is this idempotent ?
            this.gitHubApi.CreateRepositoryBranch(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                this.configurationManager.MasterRepositoryName);

            await context.Publish<IGitHubBranchCreated>(evt => evt.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }
    }
}
