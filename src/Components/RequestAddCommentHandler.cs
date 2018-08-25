namespace Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Messages.Messages;
    using NServiceBus;

    public class RequestAddCommentHandler : IHandleMessages<RequestAddComment>
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IGitHubApi gitHubApi;

        public RequestAddCommentHandler(IConfigurationManager configurationManager, IGitHubApi gitHubApi)
        {
            this.configurationManager = configurationManager;
            this.gitHubApi = gitHubApi;
        }

        public async Task Handle(RequestAddComment message, IMessageHandlerContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine(message.UserName);
            sb.Append("\t").AppendLine(message.Content);
            string content = sb.ToString();

            await this.gitHubApi.UpdateFile(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                this.configurationManager.RepositoryName,
                message.BranchName,
                message.FileName,
                content).ConfigureAwait(false);

            await context.Reply(new AddCommentResponse()).ConfigureAwait(false);
        }
    }
}
