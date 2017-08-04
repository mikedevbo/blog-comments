using Messages.Commands;
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
        public Task Handle(CreatePullRequest message, IMessageHandlerContext context)
        {


            return Task.CompletedTask;
        }
    }
}
