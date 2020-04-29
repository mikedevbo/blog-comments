using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands;
using Bc.Contracts.Internals.Endpoint.CommentRegistration.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.CommentAnswer
{
    public class CommentAddedHandler : IHandleMessages<CommentAdded>
    {
        private static readonly ILog Log = LogManager.GetLogger<CommentAddedHandler>();
        
        public Task Handle(CommentAdded message, IMessageHandlerContext context)
        {
            Log.Info($"CommentId: {message.CommentId}");
            return context.Send(new CheckCommentAnswer(message.CommentId, message.CommentUri));
        }
    }
}