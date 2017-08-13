namespace Components
{
    using System.Threading.Tasks;
    using Components.GitHub;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus;

    public class HandlerAddComment : IHandleMessages<AddComment>
    {
        private readonly IComponentsConfigurationManager componentsConfigurationManager;
        private readonly IGitHubApi gitHubApi;

        public HandlerAddComment(IComponentsConfigurationManager configurationManager, IGitHubApi gitHubApi)
        {
            this.componentsConfigurationManager = configurationManager;
            this.gitHubApi = gitHubApi;
        }

        public async Task Handle(AddComment message, IMessageHandlerContext context)
        {
            this.gitHubApi.UpdateFile(
                this.componentsConfigurationManager.UserAgent,
                this.componentsConfigurationManager.AuthorizationToken,
                this.componentsConfigurationManager.RepositoryName,
                message.BranchName,
                message.FileName,
                message.Comment);

            await context.Publish<ICommentAdded>(evt => evt.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }
    }
}
