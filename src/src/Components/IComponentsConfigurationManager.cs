namespace Components
{
    using System.Security;

    public interface IComponentsConfigurationManager
    {
        string UserAgent { get; }

        string AuthorizationToken { get; }

        string RepositoryName { get; }

        string MasterBranchName { get; }

        int CommentResponseAddedSagaTimeoutInSeconds { get; }

        string SmtpHost { get; }

        int SmtpPort { get; }

        string SmtpHostUserName { get; }

        SecureString SmtpHostPassword { get; }

        string SmtpFrom { get; }
    }
}
