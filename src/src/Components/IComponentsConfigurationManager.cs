namespace Components
{
    /// <summary>
    /// The components configuration manager interface.
    /// </summary>
    public interface IComponentsConfigurationManager
    {
        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>
        /// The user agent.
        /// </value>
        string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the authorization token.
        /// </summary>
        /// <value>
        /// The authorization token.
        /// </value>
        string AuthorizationToken { get; set; }

        /// <summary>
        /// Gets or sets the name of the repository.
        /// </summary>
        /// <value>
        /// The name of the repository.
        /// </value>
        string RepositoryName { get; set; }

        /// <summary>
        /// Gets or sets the name of the master branch.
        /// </summary>
        /// <value>
        /// The name of the master branch.
        /// </value>
        string MasterBranchName { get; set; }

        /// <summary>
        /// Gets saga timeout value indicating how often check comment response.
        /// </summary>
        /// <value>
        /// The saga timeout value indicating how often check comment response.
        /// </value>
        int CommentResponseAddedSagaTimeoutInSeconds { get; }
    }
}
