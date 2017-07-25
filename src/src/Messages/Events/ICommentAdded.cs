using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Events
{
    public interface ICommentAdded
    {
        Guid CommentId { get; set; }
    }
}
