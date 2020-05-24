using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentRegistration;
using Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands;
using Bc.Contracts.Internals.Endpoint.ITOps.Commands;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.ITOps
{
    public class TakeCommentPolicy : IHandleMessages<TakeComment>
    {
        private static readonly ILog Log = LogManager.GetLogger<TakeCommentPolicy>();
        
        public async Task Handle(TakeComment message, IMessageHandlerContext context)
        {
            await context.Send(new RegisterComment(
                new CommentData(
                    message.CommentId,
                    message.UserName,
                    message.UserWebsite,
                    message.FileName,
                    message.Content,
                    message.AddedDate))).ConfigureAwait(false);
            
            // await context.Send(new NotifyByEmailCmd(
            //     message.CommentId,
            //     message.UserEmail)).ConfigureAwait(false);
            
            Log.Info($"{this.GetType().Name}: take comment: {message.CommentId}");
        }
    }
}