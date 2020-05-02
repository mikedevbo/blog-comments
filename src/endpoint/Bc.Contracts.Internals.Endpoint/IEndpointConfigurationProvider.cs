using System.Security;

namespace Bc.Contracts.Internals.Endpoint
{
    public interface IEndpointConfigurationProvider
    {
        bool IsSendEmail { get; }
        
        string SmtpHost { get; }

        int SmtpPort { get; }

        string SmtpHostUserName { get; }

        SecureString SmtpHostPassword { get; }

        string SmtpFrom { get; }
    }
}