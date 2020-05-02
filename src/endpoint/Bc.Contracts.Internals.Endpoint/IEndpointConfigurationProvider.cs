using System.Security;

namespace Bc.Contracts.Internals.Endpoint
{
    public interface IEndpointConfigurationProvider
    {
        public string UserAgent { get; }

        public string AuthorizationToken { get; }

        public string RepositoryName { get; }

        public string MasterBranchName { get; }

        bool IsUseFakes { get; }

        bool IsSendEmail { get; }
        
        string SmtpHost { get; }

        int SmtpPort { get; }

        string SmtpHostUserName { get; }

        SecureString SmtpHostPassword { get; }

        string SmtpFrom { get; }
    }
}