using System.Threading.Tasks;
using Bc.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bc.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        // POST: api/TodoItems
        [HttpPost]
        public async Task<ActionResult<Comment>> PostTodoItem(Comment comment)
        {
            // guess what for ;)
            if (!string.IsNullOrEmpty(comment.UserPhone))
            {
                return Ok();
            }

            return Ok();
        }
    }
}