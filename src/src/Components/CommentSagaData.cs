using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class CommentSagaData : ContainSagaData
    {
        public Guid CommentId { get; set; }

        public string UserEmailAddress { get; set; }
    }
}
