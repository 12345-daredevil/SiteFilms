using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Caching.Memory;
using SiteFilms.Data;
using Microsoft.EntityFrameworkCore;

namespace SiteFilms.Services
{
    public class EmailSender(ApplicationDbContext db, IMemoryCache memory) : IEmailSender
    {
        IMemoryCache _memory { get; set; } = memory;
        ApplicationDbContext _db { get; set; } = db;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = await _db.Messages.FirstOrDefaultAsync();
            if (message == null) return;

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(message.Title, message.Email));  
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(message.URL, message.Port, false);
                await client.AuthenticateAsync(message.Login, message.Password);      
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public static async Task SendEmailAsync(Message message, string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(message.Title, message.Email));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(message.URL, message.Port, false);
                await client.AuthenticateAsync(message.Login, message.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
