using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using trading_app.dto.email;
using trading_app.interfaces;

namespace trading_app.services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config; 
        }
        public async Task<bool> SendEmailAsync(EmailSendDto emailSendDto){
            MailjetClient client = new MailjetClient(_config["MailJet:ApiKey"], _config["MailJet:SecretKey"]);

            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(_config["Email:From"], _config["Email:ApplicationName"]))
                .WithSubject(emailSendDto.Subject)
                .WithHtmlPart(emailSendDto.Body)
                .WithTo(new SendContact(emailSendDto.To))
                .Build();
            
            var response = await client.SendTransactionalEmailAsync(email);

            if (response.Messages != null)
            {
                if (response.Messages[0].Status == "success")
                {
                    return true;
                }
            }

            return false;
        }
    }
}