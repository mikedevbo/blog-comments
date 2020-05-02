using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswer;
using Bc.Contracts.Internals.Endpoint.CommentRegistration;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.CommentAnswer
{
    public class CommentAddedHandler : IHandleMessages<CommentRegisteredEvt>
    {
        private static readonly ILog Log = LogManager.GetLogger<CommentAddedHandler>();
        
        public Task Handle(CommentRegisteredEvt message, IMessageHandlerContext context)
        {
            Log.Info($"{this.GetType().Name} {message.CommentId}");
            return context.Send(new CheckCommentAnswerCmd(message.CommentId, message.CommentUri));
        }
    }
}