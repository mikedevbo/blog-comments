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
            var userName = this.FormatUserName(message.UserName, message.UserWebSite);
            sb.Append($"begin-{userName}-{message.Content}-{message.AddedDate} UTC");
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

        public string FormatUserName(string userName, string userWebSite)
        {
            return string.IsNullOrEmpty(userWebSite)
                ? $"**{userName}**"
                : $"[{userName}]({userWebSite})";
        }
    }
}
