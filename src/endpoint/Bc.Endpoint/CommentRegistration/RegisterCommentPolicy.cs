using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentRegistration;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.CommentRegistration
{
    public class RegisterCommentPolicy : IHandleMessages<RegisterCommentCmd>
    {
        private static readonly ILog Log = LogManager.GetLogger<RegisterCommentPolicy>();
        
        public Task Handle(RegisterCommentCmd message, IMessageHandlerContext context)
        {
            ////TODO: Add logic
            Log.Info($"{this.GetType().Name} {message.CommentId}");
            return context.Publish(new CommentRegisteredEvt(message.CommentId, "uri_1234"));
        }
    }
}