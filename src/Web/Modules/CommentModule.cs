namespace Web.Modules
{
    using Nancy;
    using NServiceBus;

    public class CommentModule : NancyModule
    {
        private readonly IMessageSession messageSession;

        public CommentModule(IMessageSession messageSession)
            : base("/comment")
        {
            this.messageSession = messageSession;

            this.Get["/"] = r => "aaa";

            // this.Get["/", true] = async (r, c) =>
            // {
            //    var message = new MyMessage();
            //    await messageSession.Send(message)
            //        .ConfigureAwait(false);
            //    return "Message sent to endpoint";
            // };
        }
    }
}