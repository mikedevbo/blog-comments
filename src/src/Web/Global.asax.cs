#pragma warning disable SA1649  // SA1649FileNameMustMatchTypeName
#pragma warning disable CS0649  // CS0649FileNameMustMatchTypeName

namespace Web
{
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using NServiceBus;

    /// <summary>
    /// The mvc application.
    /// </summary>
    /// <seealso cref="System.Web.HttpApplication" />
    public class MvcApplication : System.Web.HttpApplication
    {
        private IEndpointInstance endpoint;

        /// <summary>
        /// Applications the start.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            EndpointConfig.RegisterEndpoint(this.endpoint);
        }

        /// <summary>
        /// Applications the end.
        /// </summary>
        protected void Application_End()
        {
            this.endpoint?.Stop().GetAwaiter().GetResult();
        }
    }
}
