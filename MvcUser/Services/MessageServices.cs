using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;

namespace MvcUser.Services
{
    public class AuthMessageSender: ISmsSender, IEmailSender
    {
        public AuthMessageSender(IOptions<SMSoptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public SMSoptions Options { get; } //Set only via Secret Manager
        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            /*ASPSMS.SMS SMSSender = new ASPSMS.SMS();

            SMSSender.Userkey = Options.SMSAccountIdentification;
            SMSSender.Password = Options.SMSAccountPassword;
            SMSSender.Originator = Options.SMSAccountFrom;

            SMSSender.AddRecipient(number);
            SMSSender.MessageData = message;

            SMSSender.SendTextSMS();*/

            return Task.FromResult(0);
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Geometry Team", "geogeometry@mail.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.ru",465,true);
                await client.AuthenticateAsync("geogeometry@mail.ru", "geometry_123");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
