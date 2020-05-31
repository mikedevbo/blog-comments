using System.Threading.Tasks;
using Bc.Common.Endpoint;
using Bc.Contracts.Internals.Endpoint.CommentTaking.Commands;
using Bc.Contracts.Internals.Endpoint.GitHubPullRequestCreation.Messages;
using NServiceBus;

namespace Bc.Endpoint.Integration.Tests
{
    internal static class EndpointFactory
    {
        internal static Task<IEndpointInstance> GetSenderEndpoint()
        {
            const string bcEndpointAssemblyName = "Bc.Endpoint";

            var endpoint = EndpointCommon.GetEndpoint(
                "Sender.Tests",
                false,
                new EndpointCommonConfigurationProvider());

            var scanner = endpoint.AssemblyScanner();
            scanner.ExcludeAssemblies(bcEndpointAssemblyName);

            var transport = endpoint.UseTransport<SqlServerTransport>();
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(RequestCreateGitHubPullRequest).Assembly, bcEndpointAssemblyName);
            routing.RouteToEndpoint(typeof(TakeComment).Assembly, bcEndpointAssemblyName);

            return NServiceBus.Endpoint.Start(endpoint);
        }
    }
}