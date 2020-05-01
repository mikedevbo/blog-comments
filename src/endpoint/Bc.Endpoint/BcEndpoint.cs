using Bc.Common.Endpoint;
using NServiceBus;

namespace Bc.Endpoint
{
    public static class BcEndpoint
    {
        public static EndpointConfiguration GetEndpoint()
        {
            var endpoint = EndpointCommon.GetEndpoint(
                "Bc.Endpoint",
                false,
                new EndpointCommonConfigurationProvider());

            return endpoint;
        }
    }
}