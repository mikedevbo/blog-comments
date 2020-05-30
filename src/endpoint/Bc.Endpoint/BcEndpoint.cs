using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using Bc.Common.Endpoint;
using Bc.Logic.Endpoint.CommentAnswer;
using Bc.Logic.Endpoint.GitHubPullRequestCreation;
using Bc.Logic.Endpoint.GitHubPullRequestVerification;
using NServiceBus;
using NServiceBus.Persistence.Sql;

[assembly: SqlPersistenceSettings(MsSqlServerScripts = true)]  

namespace Bc.Endpoint
{
    public static class BcEndpoint
    {
        public static EndpointConfiguration GetEndpoint(IEndpointConfigurationProvider configurationProvider)
        {
            const string endpointName = "Bc.Endpoint";
            
            var endpoint = EndpointCommon.GetEndpoint(
                endpointName,
                false,
                new EndpointCommonConfigurationProvider());

            // routing
            var transport = endpoint.UseTransport<SqlServerTransport>();
            var routing = transport.Routing();
            // routing.RouteToEndpoint(
            //     assembly: typeof(TakeComment).Assembly,
            //     destination: endpointName);
            
            // dependency injection
            endpoint.RegisterComponents(reg =>
            {
                if (configurationProvider.IsUseFakes)
                {
                    reg.ConfigureComponent<GitHubPullRequestVerificationPolicyLogicFake>(DependencyLifecycle.InstancePerCall);
                    reg.ConfigureComponent<GitHubPullRequestCreationPolicyLogicFake>(DependencyLifecycle.InstancePerCall);
                    reg.ConfigureComponent<CommentAnswerPolicyLogicFake>(DependencyLifecycle.InstancePerCall);
                }
                else
                {
                    reg.ConfigureComponent<GitHubPullRequestVerificationPolicyLogic>(DependencyLifecycle.InstancePerCall);
                    reg.ConfigureComponent<GitHubPullRequestCreationPolicyLogic>(DependencyLifecycle.InstancePerCall);
                    reg.ConfigureComponent<CommentAnswerPolicyLogic>(DependencyLifecycle.InstancePerCall);
                }
            });

            // // mailer
            // var mailSettings = endpoint.EnableMailer();
            // mailSettings.UseSmtpBuilder(buildSmtpClient: () =>
            // {
            //     var smtpClient = new SmtpClient();
            //
            //     if (!configurationProvider.IsSendEmail)
            //     {
            //         var directoryLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Emails");
            //         Directory.CreateDirectory(directoryLocation);
            //         smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            //         smtpClient.PickupDirectoryLocation = directoryLocation;
            //     }
            //     else
            //     {
            //         smtpClient.Host = configurationProvider.SmtpHost;
            //         smtpClient.Port = configurationProvider.SmtpPort;
            //         smtpClient.EnableSsl = true;
            //         smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //         smtpClient.UseDefaultCredentials = false;
            //         smtpClient.Credentials = new NetworkCredential(
            //             configurationProvider.SmtpHostUserName,
            //             configurationProvider.SmtpHostPassword);
            //     }
            //
            //     return smtpClient;
            // });            

            return endpoint;
        }
    }
}