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

        public string SmtpHost
        {
            get
            {
                return ConfigurationManager.AppSettings["SmtpHost"];
            }
        }

        public int SmtpPort
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            }
        }

        public string SmtpHostUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["SmtpHostUserName"];
            }
        }

        public SecureString SmtpHostPassword
        {
            get
            {
                var pass = new SecureString();
                ConfigurationManager.AppSettings["SmtpHostPassword"]
                    .ToCharArray()
                    .ToList()
                    .ForEach(c => pass.AppendChar(c));

                return pass;
            }
        }

        public string SmtpFrom
        {
            get
            {
                return ConfigurationManager.AppSettings["SmtpFrom"];
            }
        }
    }
}
