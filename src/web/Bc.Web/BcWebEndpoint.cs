
using Bc.Common.Endpoint;
using NServiceBus;
using NServiceBus.Persistence.Sql;

[assembly: SqlPersistenceSettings(MsSqlServerScripts = true)] 

namespace Bc.Web
{
    public static class BcWebEndpoint
    {
        public static EndpointConfiguration GetEndpoint()
        {
            const string endpointName = "Bc.WebEndpoint";
            
            var endpoint = EndpointCommon.GetEndpoint(
                endpointName,
                true,
                new EndpointCommonConfigurationProvider());

            return endpoint;
        }
    }
}