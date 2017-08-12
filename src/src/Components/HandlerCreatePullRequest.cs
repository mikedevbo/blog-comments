using Components.GitHub;
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
    public class HandlerCreatePullRequest : IHandleMessages<CreatePullRequest>
    {
        private readonly IComponentsConfigurationManager componentsConfigurationManager;
        private readonly IGitHubApi gitHubApi;

        public HandlerCreatePullRequest(IComponentsConfigurationManager componentsConfigurationManager, IGitHubApi gitHubApi)
        {
            this.componentsConfigurationManager = componentsConfigurationManager;
            this.gitHubApi = gitHubApi;
        }

        public async Task Handle(CreatePullRequest message, IMessageHandlerContext context)
        {
            this.gitHubApi.CreatePullRequest(
                this.componentsConfigurationManager.UserAgent,
                this.componentsConfigurationManager.AuthorizationToken,
                this.componentsConfigurationManager.RepositoryName,
                message.HeadBranchName,
                message.BaseBranchName);

            await context.Publish<IPullRequestCreated>(evt => evt.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }
    }
}
