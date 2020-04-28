namespace Messages.Messages
{
    using System;

    public class RequestCreateBranch
    {
        public RequestCreateBranch(DateTime addedDate)
        {
            this.AddedDate = addedDate;
        }

        public DateTime AddedDate { get; }
    }
}
