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
        public Task Handle(AddComment message, IMessageHandlerContext context)
        {
            context.Publish<ICommentAdded>(evt => evt.CommentId = message.CommentId)
                .ConfigureAwait(false);

            return Task.CompletedTask;
        }
    }
}
