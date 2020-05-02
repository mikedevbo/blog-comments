using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentRegistration;
using NServiceBus;
using NServiceBus.Logging;

namespace Bc.Endpoint.CommentRegistration
{
    public class RegisterCommentPolicy :
        IHandleMessages<RegisterCommentCmd>,
        IHandleMessages<AddCommentCmd>,
        IHandleMessages<CreatePullRequestCmd>
    {
        private readonly IRegisterCommentPolicyLogic logic;
        private static readonly ILog Log = LogManager.GetLogger<RegisterCommentPolicy>();

        public RegisterCommentPolicy(IRegisterCommentPolicyLogic logic)
        {
            this.logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }
        
        public async Task Handle(RegisterCommentCmd message, IMessageHandlerContext context)
        {
            // call logic
            var branchName = await this.logic.CreateBranch(message.CommentData.AddedDate).ConfigureAwait(false);
            await context.Send(new AddCommentCmd(branchName, message.CommentData)).ConfigureAwait(false);

            Log.Info($"{this.GetType().Name}: register comment: {message.CommentData.CommentId}");
        }

        public async Task Handle(AddCommentCmd message, IMessageHandlerContext context)
        {
            // call logic
            await this.logic.AddComment(message.BranchName, message.CommentData).ConfigureAwait(false);
            await context.Send(new CreatePullRequestCmd(message.BranchName, message.CommentData)).ConfigureAwait(false);

            Log.Info($"{this.GetType().Name}: add comment: {message.CommentData.CommentId}");
        }

        public async Task Handle(CreatePullRequestCmd message, IMessageHandlerContext context)
        {
            // call logic
            var pullRequestUri = await this.logic.CreatePullRequest(message.BranchName).ConfigureAwait(false);
            await context.Publish(new CommentRegisteredEvt(message.CommentData.CommentId, pullRequestUri)).ConfigureAwait(false);

            Log.Info($"{this.GetType().Name}: create pull request: {message.CommentData.CommentId}");
        }
    }
}