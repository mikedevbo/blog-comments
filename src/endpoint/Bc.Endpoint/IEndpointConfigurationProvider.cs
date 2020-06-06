using System.Security;

namespace Bc.Endpoint
{
    public interface IEndpointConfigurationProvider
    {
        bool IsUseFakes { get; }
        
        bool IsSendEmail { get; }
        
        string SmtpHost { get; }
        
        int SmtpPort { get; }
        
        string SmtpHostUserName { get; }

        SecureString SmtpHostPassword { get; }
    }
}