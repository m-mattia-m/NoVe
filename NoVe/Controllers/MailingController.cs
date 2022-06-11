using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using MimeKit;

namespace NoVe.Controllers
{
    public class MailingController
    {

        IHostingEnvironment env = null;
        public MailingController(IHostingEnvironment env)
        {
            this.env = env;
        }

        public static void sendEmail(string toEmail, string emailMessage)
        {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress("NoVe Verifizierungscode", "nove@mattiamueggler.ch");
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress("User", toEmail);
            message.To.Add(to);

            message.Subject = "Verifizierugs Code";

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = emailMessage;

            message.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect("asmtp.mail.hostpoint.ch", 465, true);
            client.Authenticate("nove@mattiamueggler.ch", "?x9Ls!Uva77vJvic*SsF");

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
        }

    }
}
