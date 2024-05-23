using COS730.Helpers.Interfaces;
using COS730.Models.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace COS730.Helpers
{
    public class EmailHelper : IEmailHelper
    {
        private readonly EmailSettings _emailSettings;

        public EmailHelper(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public void SendEmail(string email, string subject, string message)
        {
            try
            {
                // Set up the SMTP client
                SmtpClient smtpClient = new(_emailSettings.MailServer, _emailSettings.MailPort)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password),
                    EnableSsl = true
                };

                // Create the mail message
                MailMessage mail = new()
                {
                    From = new MailAddress(_emailSettings.Sender!),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                // Add the recipient
                mail.To.Add(email);

                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }


    }
}