namespace Messages.Commands
{
    using System;

    /// <summary>
    /// The command.
    /// </summary>
    public class CreatePullRequest
    {
        /// <summary>
        /// Gets or sets the comment identifier.
        /// </summary>
        /// <value>
        /// The comment identifier.
        /// </value>
        public Guid CommentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the head branch.
        /// </summary>
        /// <value>
        /// The name of the head branch.
        /// </value>
        public string HeadBranchName { get; set; }

        /// <summary>
        /// Gets or sets the name of the base branch.
        /// </summary>
        /// <value>
        /// The name of the base branch.
        /// </value>
        public string BaseBranchName { get; set; }
    }
}
