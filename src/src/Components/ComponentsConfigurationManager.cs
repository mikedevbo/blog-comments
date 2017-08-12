namespace Components
{
    using System;

    /// <summary>
    /// The components configuration manager.
    /// </summary>
    /// <seealso cref="IComponentsConfigurationManager" />
    public class ComponentsConfigurationManager : IComponentsConfigurationManager
    {
        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>
        /// The user agent.
        /// </value>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the authorization token.
        /// </summary>
        /// <value>
        /// The authorization token.
        /// </value>
        public string AuthorizationToken { get; set; }

        /// <summary>
        /// Gets or sets the name of the repository.
        /// </summary>
        /// <value>
        /// The name of the repository.
        /// </value>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Gets or sets the name of the master branch.
        /// </summary>
        /// <value>
        /// The name of the master branch.
        /// </value>
        public string MasterBranchName { get; set; }

        /// <summary>
        /// Gets saga timeout value indicating how often check comment response.
        /// </summary>
        /// <value>
        /// The saga timeout value indicating how often check comment response.
        /// </value>
        public int CommentResponseAddedSagaTimeoutInSeconds
        {
            get
            {
                return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommentResponseAddedSagaTimeoutInSeconds"]);
            }
        }
    }
}
