using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using RiceRunner.Models;

namespace RiceRunner.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { TextBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    Console.WriteLine($"Đang kết nối đến {_emailSettings.SmtpServer}:{_emailSettings.SmtpPort}");
                    await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);

                    Console.WriteLine($"Đang xác thực với username: {_emailSettings.Username}");
                    await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);

                    Console.WriteLine($"Đang gửi email đến {toEmail}");
                    await client.SendAsync(message);

                    Console.WriteLine("Email đã gửi thành công, ngắt kết nối...");
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}