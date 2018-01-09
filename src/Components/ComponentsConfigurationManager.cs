namespace Components
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Security;

    public class ComponentsConfigurationManager : IComponentsConfigurationManager
    {
        public string UserAgent
        {
            get
            {
                return ConfigurationManager.AppSettings["UserAgent"];
            }
        }

        public string AuthorizationToken
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizationToken"];
            }
        }

        public string RepositoryName
        {
            get
            {
                return ConfigurationManager.AppSettings["RepositoryName"];
            }
        }

        public string MasterBranchName
        {
            get
            {
                return ConfigurationManager.AppSettings["MasterBranchName"];
            }
        }

        public int CommentResponseAddedSagaTimeoutInSeconds
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["CommentResponseAddedSagaTimeoutInSeconds"]);
            }
        }
    }
}
