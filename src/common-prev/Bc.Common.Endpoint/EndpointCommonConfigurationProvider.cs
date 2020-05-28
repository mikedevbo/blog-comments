using System;
using System.Configuration;

namespace Bc.Common.Endpoint
{
    public class EndpointCommonConfigurationProvider : IEndpointCommonConfigurationProvider
    {
        public string TransportConnectionString =>
            ConfigurationManager.ConnectionStrings["NsbTransportConnectionString"].ConnectionString;

        public bool IsDisableRecoverability => Convert.ToBoolean(ConfigurationManager.AppSettings["NsbIsDisableRecoverability"]);

        public bool IsSendHeartbeats => Convert.ToBoolean(ConfigurationManager.AppSettings["NsbIsSendHeartbeats"]);

        public string ServiceControlAddress => ConfigurationManager.AppSettings["NsbServiceControlAddress"];
        
        public bool IsSendMetrics => Convert.ToBoolean(ConfigurationManager.AppSettings["NsbIsSendMetrics"]);

        public string ServiceControlMetricsAddress =>
            ConfigurationManager.AppSettings["NsbServiceControlMetricsAddress"];
    }
}