using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands;
using Bc.Contracts.Internals.Endpoint.CommentTaking.Commands;
using NServiceBus;

namespace Bc.Endpoint
{
    public class CommentTakingPolicy : IHandleMessages<TakeComment>
    {
        public async Task Handle(TakeComment message, IMessageHandlerContext context)
        {
            await context.Send(new RegisterComment(
                message.CommentId,
                message.UserName,
                message.UserWebsite,
                message.UserComment,
                message.ArticleFileName,
                message.CommentAddedDate)).ConfigureAwait(false);
            
            // await context.Send(new NotifyByEmailCmd(
            //     message.CommentId,
            //     message.UserEmail)).ConfigureAwait(false);
        }
    }
}