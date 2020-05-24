using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands;
using Internals = Bc.Contracts.Internals.Endpoint.ITOps.TakeComment.Commands;
using NServiceBus;

namespace Bc.Endpoint.ITOps.TakeComment
{
    public class TakeCommentPolicy : IHandleMessages<Internals.TakeComment>
    {
        public async Task Handle(Internals.TakeComment message, IMessageHandlerContext context)
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