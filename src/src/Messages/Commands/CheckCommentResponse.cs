namespace Messages.Commands
{
    using System;

    /// <summary>
    /// The command.
    /// </summary>
    public class CheckCommentResponse
    {
        /// <summary>
        /// Gets or sets the comment identifier.
        /// </summary>
        /// <value>
        /// The comment identifier.
        /// </value>
        public Guid CommentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the branch.
        /// </summary>
        /// <value>
        /// The name of the branch.
        /// </value>
        public string BranchName { get; set; }
    }
}
