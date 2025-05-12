using FoodOrder.Application.Common.Interfaces;
using FoodOrder.Application.Common.Models;
using FoodOrder.Application.Common.Settings;
using FoodOrder.Infrastructure.Services.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace FoodOrder.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailService(EmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public async Task SendEmailAsync(EmailDto emailDto)
        {
            var message = ConvertToEmailMessage(emailDto);
            var emailMessage = CreateEmailMessage(message);
            await Send(emailMessage);
        }

        private EmailMessage ConvertToEmailMessage(EmailDto emailDto)
        {
            return new EmailMessage(emailDto.To, emailDto.Subject, emailDto.Content);
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Dinh The Anh", _emailConfiguration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };
            return emailMessage;
        }

        private async Task Send(MimeMessage emailMessage)
        {
            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    await smtpClient.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, SecureSocketOptions.SslOnConnect);
                    await smtpClient.AuthenticateAsync(_emailConfiguration.UserName, _emailConfiguration.Password);
                    await Task.Run(() => smtpClient.Send(emailMessage));
                    await smtpClient.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }
        }

    }
}
