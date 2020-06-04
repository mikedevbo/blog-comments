using System.Threading.Tasks;
using Bc.Contracts.Externals.Endpoint.CommentAnswer.Events;
using Bc.Contracts.Externals.Endpoint.CommentRegistration.Events;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands;
using Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands;
using NServiceBus;

namespace Bc.Endpoint
{
    public class EventsSubscribing :
        IHandleMessages<CommentRegistered>,
        IHandleMessages<CommentApproved>,
        IHandleMessages<CommentRejected>
    {
        public Task Handle(CommentRegistered message, IMessageHandlerContext context)
        {
            return context.Send(new CheckCommentAnswer(message.CommentId, message.CommentUri));
        }

        public Task Handle(CommentApproved message, IMessageHandlerContext context)
        {
            return context.Send(new NotifyAboutCommentAnswer(message.CommentId, true));
        }

        public Task Handle(CommentRejected message, IMessageHandlerContext context)
        {
            return context.Send(new NotifyAboutCommentAnswer(message.CommentId, false));
        }
    }
}