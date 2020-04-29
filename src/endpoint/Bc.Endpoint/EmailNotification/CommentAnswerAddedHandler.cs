using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Events;
using Bc.Contracts.Internals.Endpoint.EmailNotification.Commands;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.EmailNotification
{
    public class CommentAnswerAddedHandler : IHandleMessages<CommentAnswerAdded>
    {
        private static readonly ILog Log = LogManager.GetLogger<CommentAnswerAddedHandler>();
        
        public Task Handle(CommentAnswerAdded message, IMessageHandlerContext context)
        {
            Log.Info($"CommentId: {message.CommentId}");
            return context.Send(new NotifyAnswerByEmail(message.CommentId));
        }
    }
}