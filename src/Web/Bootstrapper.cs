namespace Web
{
    using System;
    using Common;
    using log4net;
    using log4net.Config;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;
    using NServiceBus;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Bootstrapper));

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            // initializae log
            XmlConfigurator.Configure();

            // handle errors
            pipelines.OnError += (NancyContext ctx, Exception ex) =>
            {
                Log.Error(ex.Message, ex);
                return HttpStatusCode.InternalServerError;
            };
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            // initialize endpoint
            var configurationManager = new ConfigurationManager();
            var endpointInitializer = new EndpointInitializer(configurationManager);
            var endpointConfiguration = new EndpointConfiguration(configurationManager.NsbEndpointName);
            endpointInitializer.Initialize(endpointConfiguration);
            endpointConfiguration.SendOnly();

            // start endpoint
            var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            // dependency injection
            container.Register<IMessageSession>(endpointInstance);
        }
    }
}