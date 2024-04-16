using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using SecurityAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

namespace SecurityAPI.Services
{
    public class EmailService : IEmailService
    {

        private readonly EmailSettings emailSettings;
        public EmailService(IOptions<EmailSettings> options)
        {
            this.emailSettings = options.Value;
        }



        public async Task SendEmail(string receiver, string? code, string htmlContent, string subject)
        {
            try
            {
                Mailrequest mailrequest = new Mailrequest
                {
                    ToEmail = receiver,
                    Subject = subject,
                    Body = GetHtmlcontent(code, htmlContent)
                };

                await SendEmailAsync(mailrequest);

            }
            catch (Exception)
            {
                throw;
            }
        }





        public async Task SendEmailAsync(Mailrequest mailrequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));
            email.Subject = mailrequest.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = mailrequest.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(emailSettings.Host, emailSettings.Port, false);
            smtp.Authenticate(emailSettings.Email, emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }




        private static string GetHtmlcontent(string code, string htmlContent)
        {
            string response = $"<p>{htmlContent}</p>";
            response += $"<h2>{code}</h2>";
            return response;
        }











    }
}
