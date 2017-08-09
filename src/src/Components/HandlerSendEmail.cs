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
        private readonly ISendEmail sendEmail;

        public HandlerSendEmail(ISendEmail sendEmail)
        {
            this.sendEmail = sendEmail;
        }

        public Task Handle(SendEmail message, IMessageHandlerContext context)
        {
            ///TODO: to implement -> can be awaitable ?
            this.sendEmail.Send();
            return Task.CompletedTask;
        }
    }
}
