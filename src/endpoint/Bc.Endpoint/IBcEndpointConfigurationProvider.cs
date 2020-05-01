using System.Security;

namespace Bc.Endpoint
{
    public interface IBcEndpointConfigurationProvider
    {
        bool IsSendEmail { get; }
        
        string SmtpHost { get; }

        int SmtpPort { get; }

        string SmtpHostUserName { get; }

        SecureString SmtpHostPassword { get; }

        string SmtpFrom { get; }        
    }
}