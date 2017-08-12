namespace Messages.Commands
{
    using System;

    /// <summary>
    /// The command.
    /// </summary>
    public class StartAddingComment
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
