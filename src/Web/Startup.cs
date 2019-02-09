namespace Web
{
    using System.IO;
    using Common;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Nancy.Owin;
    using NServiceBus;
    using Web.Models;

    public class Startup
    {
        private readonly IConfiguration config;
        private IEndpointInstance endpointInstance;

        public Startup(IHostingEnvironment env)
        {
            this.config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.web.dev.json", true, true)
                            .AddJsonFile("appsettings.web.test.json", true, true)
                            .AddJsonFile("appsettings.web.production.json", true, true)
                            .Build();
        }

        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime)
        {
            // initialize endpoint
            var configurationManager = new ConfigurationManager(this.config);
            var endpointInitializer = new EndpointInitializer(configurationManager);
            var endpointConfiguration = new EndpointConfiguration(configurationManager.NsbEndpointName);
            endpointInitializer.Initialize(endpointConfiguration, true);
            applicationLifetime.ApplicationStopping.Register(this.OnShutdown);

            // start endpoint
            this.endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            var commentValidator = new CommentValidator();

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new Bootstrapper(this.endpointInstance, commentValidator)));
        }

        private void OnShutdown()
        {
            this.endpointInstance?.Stop().GetAwaiter().GetResult();
        }
    }
}
