using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.CommentRegistration
{
    public class RegisterCommentHandler : IHandleMessages<RegisterComment>
    {
        private static readonly ILog Log = LogManager.GetLogger<RegisterCommentHandler>();
        
        public Task Handle(RegisterComment message, IMessageHandlerContext context)
        {
            ////TODO: Add logic
            return Task.CompletedTask;
        }
    }
}