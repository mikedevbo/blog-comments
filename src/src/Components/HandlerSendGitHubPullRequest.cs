using Messages.Commands;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class HandlerSendGitHubPullRequest : IHandleMessages<SendGitHubPullRequest>
    {
        public Task Handle(SendGitHubPullRequest message, IMessageHandlerContext context)
        {


            return Task.CompletedTask;
        }
    }
}
