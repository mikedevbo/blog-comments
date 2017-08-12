namespace Messages.Commands
{
    using System;

    /// <summary>
    /// The command.
    /// </summary>
    public class CreateBranch
    {
        /// <summary>
        /// Gets or sets the comment identifier.
        /// </summary>
        /// <value>
        /// The comment identifier.
        /// </value>
        public Guid CommentId { get; set; }
    }
}
