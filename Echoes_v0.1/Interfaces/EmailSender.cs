using Microsoft.AspNetCore.Identity.UI.Services;

namespace Echoes_v0._1.Interfaces
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string body)
        {
            return Task.CompletedTask;
        }

    }
}
