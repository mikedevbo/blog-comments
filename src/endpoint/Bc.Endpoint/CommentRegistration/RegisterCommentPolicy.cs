using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentRegistration;
using Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.CommentRegistration
{
    public class RegisterCommentPolicy :
        IHandleMessages<RegisterComment>,
        IHandleMessages<AddComment>,
        IHandleMessages<CreatePullRequest>
    {
        private readonly IRegisterCommentPolicyLogic logic;
        private static readonly ILog Log = LogManager.GetLogger<RegisterCommentPolicy>();

        public RegisterCommentPolicy(IRegisterCommentPolicyLogic logic)
        {
            this.logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }
        
        public async Task Handle(RegisterComment message, IMessageHandlerContext context)
        {
            var branchName = await this.logic.CreateBranch(message.CommentData.AddedDate).ConfigureAwait(false);
            await context.Send(new AddComment(branchName, message.CommentData)).ConfigureAwait(false);

            Log.Info($"{this.GetType().Name}: register comment: {message.CommentData.CommentId}");
        }

        public async Task Handle(AddComment message, IMessageHandlerContext context)
        {
            await this.logic.AddComment(message.BranchName, message.CommentData).ConfigureAwait(false);
            await context.Send(new CreatePullRequest(message.BranchName, message.CommentData)).ConfigureAwait(false);

            Log.Info($"{this.GetType().Name}: add comment: {message.CommentData.CommentId}");
        }

        public async Task Handle(CreatePullRequest message, IMessageHandlerContext context)
        {
            var pullRequestUri = await this.logic.CreatePullRequest(message.BranchName).ConfigureAwait(false);
            //await context.Publish(new CommentRegistered(message.CommentData.CommentId, pullRequestUri)).ConfigureAwait(false);

            Log.Info($"{this.GetType().Name}: create pull request: {message.CommentData.CommentId}");
        }
    }
}