using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentRegistration;
using Bc.Contracts.Internals.Endpoint.EmailNotification;
using Bc.Contracts.Internals.Endpoint.Operations;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.Operations
{
    public class TakeCommentPolicy : IHandleMessages<TakeCommentCmd>
    {
        private static readonly ILog Log = LogManager.GetLogger<TakeCommentPolicy>();
        
        public async Task Handle(TakeCommentCmd message, IMessageHandlerContext context)
        {
            await context.Send(new RegisterCommentCmd(
                message.CommentId,
                message.UserName,
                message.UserWebsite,
                message.FileName,
                message.Content,
                message.AddedDate)).ConfigureAwait(false);
            
            await context.Send(new NotifyByEmailCmd(
                message.CommentId,
                message.UserEmail)).ConfigureAwait(false);
            
            Log.Info($"Take comment data. CommentId: {message.CommentId}");
        }
    }
}