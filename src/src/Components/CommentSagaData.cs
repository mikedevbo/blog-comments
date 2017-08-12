namespace Components
{
    using System;
    using NServiceBus;

    /// <summary>
    /// The saga data.
    /// </summary>
    /// <seealso cref="NServiceBus.ContainSagaData" />
    public class CommentSagaData : ContainSagaData
    {
        /// <summary>
        /// Gets or sets the comment identifier.
        /// </summary>
        /// <value>
        /// The comment identifier.
        /// </value>
        public Guid CommentId { get; set; }

        /// <summary>
        /// Gets or sets the user email address.
        /// </summary>
        /// <value>
        /// The user email address.
        /// </value>
        public string UserEmailAddress { get; set; }
    }
}
