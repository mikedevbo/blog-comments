using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class SendEmailForTests : ISendEmail
    {
        private ILog log = LogManager.GetLogger<SendEmailForTests>();

        public void Send()
        {
            log.Info("send e-mail");
        }
    }
}
