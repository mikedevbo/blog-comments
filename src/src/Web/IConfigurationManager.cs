namespace Web
{
    /// <summary>
    /// The configuration manager interface.
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets the name of the NSB endpoint.
        /// </summary>
        /// <value>
        /// The name of the NSB endpoint.
        /// </value>
        string NsbEndpointName { get; }

        /// <summary>
        /// Gets the NSB endpoint instance identifier.
        /// </summary>
        /// <value>
        /// The NSB endpoint instance identifier.
        /// </value>
        string NsbEndpointInstanceId { get; }

        /// <summary>
        /// Gets the NSB transport connection string.
        /// </summary>
        /// <value>
        /// The NSB transport connection string.
        /// </value>
        string NsbTransportConnectionString { get; }

        /// <summary>
        /// Gets the name of the NSB error queue.
        /// </summary>
        /// <value>
        /// The name of the NSB error queue.
        /// </value>
        string NsbErrorQueueName { get; }

        /// <summary>
        /// Gets the name of the NSB audit queue.
        /// </summary>
        /// <value>
        /// The name of the NSB audit queue.
        /// </value>
        string NsbAuditQueueName { get; }

        /// <summary>
        /// Gets a value indicating whether [NSB is integration tests].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [NSB is integration tests]; otherwise, <c>false</c>.
        /// </value>
        bool NsbIsIntegrationTests { get; }

        /// <summary>
        /// Gets the dev mode.
        /// </summary>
        /// <value>
        /// The dev mode.
        /// </value>
        DevMode DevMode { get; }
    }
}
