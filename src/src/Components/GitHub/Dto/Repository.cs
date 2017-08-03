using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.GitHub.Dto
{
    public class Repository
    {
        public string Ref { get; set; }

        public string Url { get; set; }

        public Object Object { get; set; }
    }
}
