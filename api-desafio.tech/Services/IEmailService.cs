using DotNetEnv;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace api_desafio.tech.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;

        public EmailService()
        {
            Env.Load();

            _smtpServer = Env.GetString("SMTP_SERVER");
            _smtpPort = Env.GetInt("SMTP_PORT");
            _smtpUser = Env.GetString("SMTP_USER");
            _smtpPass = Env.GetString("SMTP_PASS");
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            using var smtpClient = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
