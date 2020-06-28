
using Bc.Common.Endpoint;
using Bc.Contracts.Internals.Endpoint.CommentTaking.Commands;
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
            const string destinationEndpointName = "Bc.Endpoint";
            
            var endpoint = EndpointCommon.GetEndpoint(
                endpointName,
                true,
                new EndpointCommonConfigurationProvider());

            var transport = endpoint.UseTransport<SqlServerTransport>();
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(TakeComment).Assembly, destinationEndpointName);

            return endpoint;
        }
    }
}