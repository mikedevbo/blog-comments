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
    public class HandlerCheckCommentResponse : IHandleMessages<CheckCommentResponse>
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IGitHubApi gitHubApi;

        public HandlerCheckCommentResponse(IConfigurationManager configurationManager, IGitHubApi gitHubApi)
        {
            this.configurationManager = configurationManager;
            this.gitHubApi = gitHubApi;
        }

        public async Task Handle(CheckCommentResponse message, IMessageHandlerContext context)
        {
            var repo = this.gitHubApi.GetRepository(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                this.configurationManager.RepositoryName,
                message.BranchName);

            ////TODO: add if implementation

            await context.Publish<ICommentResponseAdded>(evt => evt.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }
    }
}
