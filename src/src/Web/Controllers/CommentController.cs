namespace Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Messages.Commands;
    using NServiceBus;
    using Web.Models;

    /// <summary>
    /// The controller.
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public class CommentController : Controller
    {
        private IEndpointInstance endpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentController"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        public CommentController(IEndpointInstance endpoint)
        {
            this.endpoint = endpoint;
        }

        /// <summary>
        /// Requests for comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>Represents the async method.</returns>
        [HttpPost]
        public async Task RequestForComment(Comment comment)
        {
            var sendOptions = new SendOptions();

            await this.endpoint.Send<StartAddingComment>(cmd => cmd.CommentId = comment.Id)
                .ConfigureAwait(false);
        }
    }
}