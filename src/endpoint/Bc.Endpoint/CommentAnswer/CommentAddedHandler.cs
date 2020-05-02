using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswer;
using Bc.Contracts.Internals.Endpoint.CommentRegistration;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.CommentAnswer
{
    public class CommentAddedHandler : IHandleMessages<CommentAddedEvt>
    {
        private static readonly ILog Log = LogManager.GetLogger<CommentAddedHandler>();
        
        public Task Handle(CommentAddedEvt message, IMessageHandlerContext context)
        {
            Log.Info($"CommentId: {message.CommentId}");
            return context.Send(new CheckCommentAnswerCmd(message.CommentId, message.CommentUri));
        }
    }
}