namespace Messages.Events
{
    using System;

    /// <summary>
    /// The event.
    /// </summary>
    public interface ICommentResponseAdded
    {
        /// <summary>
        /// Gets or sets the comment identifier.
        /// </summary>
        /// <value>
        /// The comment identifier.
        /// </value>
        Guid CommentId { get; set; }

        /// <summary>
        /// Gets or sets the state of the comment response.
        /// </summary>
        /// <value>
        /// The state of the comment response.
        /// </value>
        CommentResponseState CommentResponseState { get; set; }
    }
}
