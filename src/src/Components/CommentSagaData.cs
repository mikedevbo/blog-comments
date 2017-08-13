namespace Components
{
    using System;
    using NServiceBus;

    public class CommentSagaData : ContainSagaData
    {
        public Guid CommentId { get; set; }

        public string UserEmailAddress { get; set; }
    }
}
