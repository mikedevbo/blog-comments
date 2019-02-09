namespace Web
{
    using FluentValidation;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;
    using NServiceBus;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IMessageSession messageSession;
        private readonly IValidator validator;

        public Bootstrapper(IMessageSession messageSession, IValidator validator)
        {
            this.messageSession = messageSession;
            this.validator = validator;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            // initializae log
            //XmlConfigurator.Configure();

            // handle errors
            //pipelines.OnError += (NancyContext ctx, Exception ex) =>
            //{
            //    Log.Error(ex.Message, ex);
            //    return HttpStatusCode.InternalServerError;
            //};
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