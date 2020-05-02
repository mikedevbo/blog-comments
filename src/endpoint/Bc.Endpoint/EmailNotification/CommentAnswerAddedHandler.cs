using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswer;
using Bc.Contracts.Internals.Endpoint.EmailNotification;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.EmailNotification
{
    public class CommentAnswerAddedHandler : IHandleMessages<CommentAnswerAddedEvt>
    {
        private static readonly ILog Log = LogManager.GetLogger<CommentAnswerAddedHandler>();
        
        public Task Handle(CommentAnswerAddedEvt message, IMessageHandlerContext context)
        {
            Log.Info($"{this.GetType().Name} {message.CommentId}");
            return context.Send(new NotifyAnswerByEmailCmd(message.CommentId, message.IsApproved));
        }
    }
}