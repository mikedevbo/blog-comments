using NServiceBus.Logging;
using Simple.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class SendEmailForTests : ISendEmail
    {
        private static ILog log = LogManager.GetLogger<SendEmailForTests>();
        private readonly IConfigurationManager configurationManager;

        public SendEmailForTests(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public void Send()
        {
            Database.OpenConnection(this.configurationManager.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 5);

            log.Info("send e-mail");
        }
    }
}
