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
    public class HandlerCheckCommentResponse : IHandleMessages<CheckCommentResponse>
    {
        public Task Handle(CheckCommentResponse message, IMessageHandlerContext context)
        {
            context.Publish<ICommentResponseAdded>(evt => evt.CommentId = message.CommentId)
                .ConfigureAwait(false);

            return Task.CompletedTask;
        }
    }
}
