using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands;
using Bc.Contracts.Internals.Endpoint.EmailNotification.Commands;
using Bc.Contracts.Internals.Endpoint.Operations.Commands;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.Operations
{
    public class TakeCommentPolicy : IHandleMessages<TakeComment>
    {
        private static readonly ILog Log = LogManager.GetLogger<TakeCommentPolicy>();
        
        public async Task Handle(TakeComment message, IMessageHandlerContext context)
        {
            await context.Send(new RegisterComment(
                message.CommentId,
                message.UserName,
                message.UserWebsite,
                message.FileName,
                message.Content,
                message.AddedDate)).ConfigureAwait(false);

            await context.Send(new NotifyByEmail(
                message.CommentId,
                message.UserEmail)).ConfigureAwait(false);
            
            Log.Info($"Take comment data. CommentId: {message.CommentId}");
        }
    }
}