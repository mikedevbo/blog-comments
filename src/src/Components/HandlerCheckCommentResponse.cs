namespace Components
{
    using System;
    using System.Threading.Tasks;
    using Components.GitHub;
    using Messages;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus;

    public class HandlerCheckCommentResponse : IHandleMessages<CheckCommentResponse>
    {
        private readonly IComponentsConfigurationManager componentsConfigurationManager;
        private readonly IGitHubApi gitHubApi;

        public HandlerCheckCommentResponse(IComponentsConfigurationManager componentsConfigurationManager, IGitHubApi gitHubApi)
        {
            this.componentsConfigurationManager = componentsConfigurationManager;
            this.gitHubApi = gitHubApi;
        }

        public Task Handle(CheckCommentResponse message, IMessageHandlerContext context)
        {
            CommentResponseState responseState = CommentResponseState.Approved;

            try
            {
                var repo = this.gitHubApi.GetRepository(
                    this.componentsConfigurationManager.UserAgent,
                    this.componentsConfigurationManager.AuthorizationToken,
                    this.componentsConfigurationManager.RepositoryName,
                    message.BranchName);

                responseState = CommentResponseState.NotAddded;
            }
            catch (Exception)
            {
                ////TODO: add proper exception type to recognize that there is no branch
            }

            return context.Publish<ICommentResponseAdded>(
                evt =>
                {
                    evt.CommentId = message.CommentId;
                    evt.CommentResponseState = responseState;
                });
        }
    }
}
