namespace Components
{
    using System.Threading.Tasks;
    using Messages.Commands;
    using NServiceBus;

    /// <summary>
    /// The message handler.
    /// </summary>
    public class HandlerSendEmail : IHandleMessages<SendEmail>
    {
        private readonly ISendEmail sendEmail;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerSendEmail"/> class.
        /// </summary>
        /// <param name="sendEmail">The send email.</param>
        public HandlerSendEmail(ISendEmail sendEmail)
        {
            this.sendEmail = sendEmail;
        }

        /// <summary>
        /// Handles a message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="context">The context of the currently handled message.</param>
        /// <returns>Represents the message handler.</returns>
        /// <remarks>
        /// This method will be called when a message arrives on at the endpoint and should contain
        /// the custom logic to execute when the message is received.
        /// </remarks>
        public Task Handle(SendEmail message, IMessageHandlerContext context)
        {
            ////TODO: to implement -> can be awaitable ?
            this.sendEmail.Send();
            return Task.CompletedTask;
        }
    }
}
