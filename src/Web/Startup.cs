namespace Web
{
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Nancy.Owin;

    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IHostingEnvironment env)
        {
            this.config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.web.dev.json", true, true)
                            .AddJsonFile("appsettings.web.test.json", true, true)
                            .AddJsonFile("appsettings.web.production.json", true, true)
                            .Build();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new Bootstrapper(this.config)));
        }
    }
}
