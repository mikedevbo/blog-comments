namespace Web
{
    using System;

    /// <summary>
    /// The configuration manager.
    /// </summary>
    /// <seealso cref="Web.IConfigurationManager" />
    public class ConfigurationManager : IConfigurationManager
    {
        /// <summary>
        /// Gets the name of the NSB endpoint.
        /// </summary>
        /// <value>
        /// The name of the NSB endpoint.
        /// </value>
        public string NsbEndpointName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[@"NsbEndpointName"];
            }
        }

        /// <summary>
        /// Gets the NSB endpoint instance identifier.
        /// </summary>
        /// <value>
        /// The NSB endpoint instance identifier.
        /// </value>
        public string NsbEndpointInstanceId
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[@"NsbEndpointInstanceId"];
            }
        }

        /// <summary>
        /// Gets the NSB transport connection string.
        /// </summary>
        /// <value>
        /// The NSB transport connection string.
        /// </value>
        public string NsbTransportConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings[@"NsbTransportConnectionString"].ConnectionString;
            }
        }

        /// <summary>
        /// Gets the name of the NSB error queue.
        /// </summary>
        /// <value>
        /// The name of the NSB error queue.
        /// </value>
        public string NsbErrorQueueName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[@"NsbErrorQueueName"];
            }
        }

        /// <summary>
        /// Gets the name of the NSB audit queue.
        /// </summary>
        /// <value>
        /// The name of the NSB audit queue.
        /// </value>
        public string NsbAuditQueueName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[@"NsbAuditQueueName"];
            }
        }

        /// <summary>
        /// Gets a value indicating whether [NSB is integration tests].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [NSB is integration tests]; otherwise, <c>false</c>.
        /// </value>
        public bool NsbIsIntegrationTests
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["NsbIsIntegrationTests"]);
            }
        }

        /// <summary>
        /// Gets the dev mode.
        /// </summary>
        /// <value>
        /// The dev mode.
        /// </value>
        /// <exception cref="ArgumentException">dev mode not implemented: {0}</exception>
        public DevMode DevMode
        {
            get
            {
                var devMode = System.Configuration.ConfigurationManager.AppSettings["DevMode"];

                switch (devMode)
                {
                    case "dev":
                        return DevMode.Dev;

                    case "test":
                        return DevMode.Test;

                    case "production":
                        return DevMode.Production;

                    default:
                        throw new ArgumentException("dev mode not implemented: {0}", devMode);
                }
            }
        }
    }
}