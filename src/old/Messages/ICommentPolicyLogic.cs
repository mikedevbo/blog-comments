namespace Messages
{
    using System;
    using System.Threading.Tasks;

    public interface ICommentPolicyLogic
    {
        Task<Messages.CreateBranchResponse> CreateRepositoryBranch(Messages.RequestCreateBranch message);

        Task<Messages.AddCommentResponse> UpdateFile(Messages.RequestAddComment message);

        Task<Messages.CreatePullRequestResponse> CreatePullRequest(Messages.RequestCreatePullRequest message);

        Task<Messages.CheckCommentAnswerResponse> CheckCommentAnswer(Messages.RequestCheckCommentAnswer message);
    }
}