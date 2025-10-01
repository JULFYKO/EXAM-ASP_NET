using System;
using System.Threading.Tasks;
using EXAM_ASP_NET.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;

namespace EXAM_ASP_NET.Services
{
    public class EmailOptions
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 25;
        public bool UseSsl { get; set; } = false;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = "no-reply@example.com";
        public string FromName { get; set; } = "EXAM";
    }

    public class MailKitEmailSender : IAppEmailSender
    {
        private readonly EmailOptions _opts;
        private readonly ILogger<MailKitEmailSender> _log;

        public MailKitEmailSender(IOptions<EmailOptions> opts, ILogger<MailKitEmailSender> log)
        {
            _opts = opts.Value;
            _log = log;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_opts.FromName, _opts.FromEmail));
            msg.To.Add(MailboxAddress.Parse(toEmail));
            msg.Subject = subject;
            var body = new BodyBuilder { HtmlBody = htmlMessage };
            msg.Body = body.ToMessageBody();

            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(_opts.Host, _opts.Port, _opts.UseSsl);
                if (!string.IsNullOrEmpty(_opts.Username))
                {
                    await client.AuthenticateAsync(_opts.Username, _opts.Password);
                }
                await client.SendAsync(msg);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to send email to {to}", toEmail);
                throw;
            }
        }
    }
}
