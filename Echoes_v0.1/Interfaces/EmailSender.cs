using Microsoft.AspNetCore.Identity.UI.Services;

namespace Echoes_v0._1.Interfaces
{
    public class EmailSender : IEmailSender
    {
        //Temp
        public Task SendEmailAsync(string email, string subject, string body)
        {
            return Task.CompletedTask; //marks email confirmation as completed
        }

        ///SendGrid documentation
        //private readonly EmailSenderOptions _emailSenderOptions;

        //public EmailSender(IOptions<EmailSenderOptions> emailSenderOptions)
        //{
        //    _emailSenderOptions = emailSenderOptions.Value;
        //}

        //public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        //{
        //    var apiKey = _emailSenderOptions.SendGridApiKey; // You can use your preferred email provider here
        //    var client = new SendGridClient(apiKey);
        //    var from = new EmailAddress(_emailSenderOptions.FromEmail, _emailSenderOptions.FromName);
        //    var to = new EmailAddress(email);
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlMessage);
        //    var response = await client.SendEmailAsync(msg);
        //}


    }
}
