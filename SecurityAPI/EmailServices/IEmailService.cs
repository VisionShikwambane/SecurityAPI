using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using SecurityAPI.Models;

namespace SecurityAPI.Services
{
    public interface IEmailService

    {
        Task SendEmailAsync(Mailrequest mailrequest);
        Task SendEmail(string receiver, string? code, string htmlContent, string subject);
    }
}
