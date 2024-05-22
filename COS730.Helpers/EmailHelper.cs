using System.Net;
using System.Net.Mail;

namespace COS730.Helpers
{
    public static class EmailHelper
    {
        public static void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                // Set up the SMTP client
                SmtpClient smtpClient = new("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("marmap4@gmail.com", "uongissnirufnvbi"),
                    EnableSsl = true
                };

                // Create the mail message
                MailMessage mail = new()
                {
                    From = new MailAddress("marmap4@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                // Add the recipient
                mail.To.Add(toEmail);

                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email. Error: {ex.Message}");
            }
        }

    }
}