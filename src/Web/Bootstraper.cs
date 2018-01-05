namespace Web
{
    using System.Data.SqlClient;
    using Common;
    using Messages.Commands;
    using Messages.Events;
    using Nancy;
    using Nancy.TinyIoc;
    using NServiceBus;
    using NServiceBus.Persistence.Sql;

    public class Bootstraper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            var configurationManager = new ConfigurationManager();
            var endpointInitializer = new EndpointInitializer(configurationManager);
            var endpointConfiguration = new EndpointConfiguration(configurationManager.NsbEndpointName);

            // initialize endpoint
            endpointInitializer.Initialize(endpointConfiguration);
            endpointConfiguration.SendOnly();

            // start endpoint
            var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            container.Register<IMessageSession>(endpointInstance);
        }
    }
}