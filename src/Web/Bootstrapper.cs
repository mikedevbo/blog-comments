namespace Web
{
    using System;
    using FluentValidation;
    using Microsoft.Extensions.Logging;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;
    using NServiceBus;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IMessageSession messageSession;
        private readonly IValidator validator;
        private readonly ILogger logger;

        public Bootstrapper(IMessageSession messageSession, IValidator validator, ILogger logger)
        {
            this.messageSession = messageSession;
            this.validator = validator;
            this.logger = logger;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            // handle errors
            pipelines.OnError += (NancyContext ctx, Exception ex) =>
            {
                this.logger.LogError(ex, ex.Message);
                return HttpStatusCode.InternalServerError;
            };
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            // dependency injection
            container.Register(this.messageSession);
            container.Register(this.validator);
        }
    }
}