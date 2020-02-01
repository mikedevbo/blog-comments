namespace Host.CommentPolicy
{
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NServiceBus;

    public class Startup
    {
        private IEndpointInstance endpointInstance;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = EndpointManager.BuildConfiguration(Directory.GetCurrentDirectory());
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            this.endpointInstance = EndpointManager.StartEndpoint(this.Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            applicationLifetime.ApplicationStopping.Register(this.OnShutdown);
        }

        private void OnShutdown()
        {
            this.endpointInstance?.Stop().GetAwaiter().GetResult();
        }
    }
}
