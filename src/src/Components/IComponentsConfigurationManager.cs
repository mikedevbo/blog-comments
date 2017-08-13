namespace Components
{
    public interface IComponentsConfigurationManager
    {
        string UserAgent { get; set; }

        string AuthorizationToken { get; set; }

        string RepositoryName { get; set; }

        string MasterBranchName { get; set; }

        int CommentResponseAddedSagaTimeoutInSeconds { get; }
    }
}
