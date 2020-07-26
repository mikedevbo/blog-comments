using System;
using System.Configuration;
using System.Linq;
using System.Security;

namespace Bc.Endpoint
{
    public class EndpointConfigurationProvider : IEndpointConfigurationProvider
    {
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
    }
}