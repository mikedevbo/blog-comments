using System;
using System.Configuration;
using System.Linq;
using System.Security;
using Bc.Contracts.Internals.Endpoint;

namespace Bc.Endpoint
{
    public class EndpointConfigurationProvider : IEndpointConfigurationProvider
    {
        public string UserAgent => ConfigurationManager.AppSettings["UserAgent"];

        public string AuthorizationToken => ConfigurationManager.AppSettings["AuthorizationToken"];

        public string RepositoryName => ConfigurationManager.AppSettings["RepositoryName"];

        public string MasterBranchName => ConfigurationManager.AppSettings["MasterBranchName"];

        public bool IsUseFakes => Convert.ToBoolean(ConfigurationManager.AppSettings["IsUseFakes"]);

        public bool IsSendEmail => Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendEmail"]);
        
        public string SmtpHost => ConfigurationManager.AppSettings["SmtpHost"];
        
        public int SmtpPort => Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
        
        public string SmtpHostUserName => ConfigurationManager.AppSettings["SmtpHostUserName"];

        public SecureString SmtpHostPassword
        {
            get
            {
                var pass = new SecureString();
                ConfigurationManager.AppSettings["SmtpHostPassword"]
                    .ToList()
                    .ForEach(c => pass.AppendChar(c));

                return pass;
            }
        }
        
        public string SmtpFrom => ConfigurationManager.AppSettings["SmtpFrom"];

        public int CheckCommentAnswerTimeoutInSeconds =>
            Convert.ToInt32(ConfigurationManager.AppSettings["CheckCommentAnswerTimeoutInSeconds"]);
    }
}