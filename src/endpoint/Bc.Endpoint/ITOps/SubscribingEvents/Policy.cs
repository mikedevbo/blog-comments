using System.Threading.Tasks;
using Bc.Contracts.Externals.Endpoint.CommentRegistration.Events;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands;
using NServiceBus;

namespace Bc.Endpoint.ITOps.SubscribingEvents
{
    public class Policy :
        IHandleMessages<CommentRegistered>
    {
        public Task Handle(CommentRegistered message, IMessageHandlerContext context)
        {
            return context.Send(new CheckCommentAnswer(message.CommentId, message.CommentUri));
        }
    }
}