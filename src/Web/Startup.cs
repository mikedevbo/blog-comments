namespace Web
{
    using System.IO;
    using Common;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
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

        public void ConfigureServices(IServiceCollection services)
        {
            ////services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            ////{
            ////    builder.AllowAnyOrigin()
            ////           .AllowAnyMethod()
            ////           .AllowAnyHeader();
            ////}));
        }

        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
        {
            ////app.UseCors("MyPolicy");

            // initialize endpoint
            var configurationManager = new ConfigurationManager(this.config);
            var endpointInitializer = new EndpointInitializer(configurationManager);
            var endpointConfiguration = new EndpointConfiguration(configurationManager.NsbEndpointName);
            endpointInitializer.Initialize(endpointConfiguration, true);
            applicationLifetime.ApplicationStopping.Register(this.OnShutdown);

            // start endpoint
            this.endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            // initialize validator
            var commentValidator = new CommentValidator();

            // initialize logger
            var legger = loggerFactory.AddLog4Net().CreateLogger("Web");

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new Bootstrapper(
                this.endpointInstance,
                commentValidator,
                legger)));
        }

        private void OnShutdown()
        {
            this.endpointInstance?.Stop().GetAwaiter().GetResult();
        }
    }
}
