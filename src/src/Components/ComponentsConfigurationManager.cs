using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class ComponentsConfigurationManager : IComponentsConfigurationManager
    {
        public string UserAgent { get; set; }

        public string AuthorizationToken { get; set; }

        public string RepositoryName { get; set; }

        public string MasterBranchName { get; set; }

        public int CommentResponseAddedSagaTimeoutInSeconds
        {
            get
            {
                return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommentResponseAddedSagaTimeoutInSeconds"]);
            }
        }
    }
}
