using System;
using System.Threading.Tasks;
using Bc.Contracts.Externals.Endpoint.CommentRegistration.Events;
using Bc.Contracts.Internals.Endpoint.CommentAnswer;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.CommentAnswer
{
    public class PolicyHandlers :
        IHandleMessages<CommentRegistered>,
        IHandleMessages<RequestCheckCommentAnswer>
    {
        private readonly ICommentAnswerPolicyLogic logic;

        public PolicyHandlers(ICommentAnswerPolicyLogic logic)
        {
            this.logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }
        
        public Task Handle(CommentRegistered message, IMessageHandlerContext context)
        {
            return context.Send(new CheckCommentAnswer(message.CommentId, message.CommentUri));
        }

        public async Task Handle(RequestCheckCommentAnswer message, IMessageHandlerContext context)
        {
            var response = await this.logic.CheckAnswer(message.CommentUri, message.Etag).ConfigureAwait(false);
            await context.Reply(response).ConfigureAwait(false);
        }
    }
}