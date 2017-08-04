using Messages.Commands;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class HandlerSendEmail : IHandleMessages<SendEmail>
    {
        public Task Handle(SendEmail message, IMessageHandlerContext context)
        {
            ///TODO: to implement
            return Task.CompletedTask;
        }
    }
}
