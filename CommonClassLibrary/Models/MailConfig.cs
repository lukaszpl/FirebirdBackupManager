using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClassLibrary.Models
{
    public class MailConfig
    {
        public MailConfig(string smtpServer, string smtpPort, string smtpUserName, string smtpPassword, bool? SSL, string receiver)
        {
            this.smtpServer = smtpServer;
            this.smtpPort = smtpPort;
            this.smtpUserName = smtpUserName;
            this.smtpPassword = smtpPassword;
            this.SSL = SSL;
            this.receiver = receiver;
        }

        public string smtpServer { get; set; }
        public string smtpPort { get; set; }
        public string smtpUserName { get; set; }
        public string smtpPassword { get; set; }
        public bool? SSL {  get; set; }
        public string receiver {  get; set; }

    }
}
