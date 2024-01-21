using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CommonClassLibrary
{
    public class MailSender
    {
        public static string SendEmail(string server, int port, string user, string password, bool? SSL, string receiver, string subject, string content)
        {
            //create SMTP client
            SmtpClient smtpClient = new SmtpClient(server, port)
            {
                Credentials = new NetworkCredential(user, password),
                EnableSsl = (bool)SSL
            };

            //new email
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(user, "Firebird backup manager"),
                Subject = subject,
                Body = content
            };

            mailMessage.To.Add(new MailAddress(receiver));


            try
            {
                smtpClient.Send(mailMessage);
                return "true";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
