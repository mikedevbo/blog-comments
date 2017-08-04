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
    public class HandlerAddComment : IHandleMessages<AddComment>
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IGitHubApi gitHubApi;

        public HandlerAddComment(IConfigurationManager configurationManager, IGitHubApi gitHubApi)
        {
            this.configurationManager = configurationManager;
            this.gitHubApi = gitHubApi;
        }

        public async Task Handle(AddComment message, IMessageHandlerContext context)
        {


            await context.Publish<ICommentAdded>(evt => evt.CommentId = message.CommentId)
                .ConfigureAwait(false);
        }
    }
}
