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
        public Task Handle(CreateGitHubBranch message, IMessageHandlerContext context)
        {
            ////Call github api
            context.Publish<IGitHubBranchCreated>(evt => evt.CommentId = message.CommentId)
                .ConfigureAwait(false);

            return Task.CompletedTask;
        }
    }
}
