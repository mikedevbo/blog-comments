using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswer;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.CommentAnswer
{
    public class RequestCheckCommentAnswerHandler : IHandleMessages<RequestCheckCommentAnswerMsg>
    {
        private static readonly ILog Log = LogManager.GetLogger<RequestCheckCommentAnswerHandler>();
        private readonly ICommentAnswerPolicyLogic logic;

        public RequestCheckCommentAnswerHandler(ICommentAnswerPolicyLogic logic)
        {
            this.logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }
        
        public async Task Handle(RequestCheckCommentAnswerMsg message, IMessageHandlerContext context)
        {
            var response = await this.logic.CheckAnswer(message.CommentUri, message.Etag).ConfigureAwait(false);
            await context.Reply(response).ConfigureAwait(false);
        }
    }
}