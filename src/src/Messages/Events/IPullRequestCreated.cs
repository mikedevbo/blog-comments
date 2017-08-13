namespace Messages.Events
{
    using System;

    public interface IPullRequestCreated
    {
        Guid CommentId { get; set; }
    }
}
