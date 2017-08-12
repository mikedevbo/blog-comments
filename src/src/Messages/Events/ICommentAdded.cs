namespace Messages.Events
{
    using System;

    /// <summary>
    /// The event.
    /// </summary>
    public interface ICommentAdded
    {
        /// <summary>
        /// Gets or sets the comment identifier.
        /// </summary>
        /// <value>
        /// The comment identifier.
        /// </value>
        Guid CommentId { get; set; }
    }
}
