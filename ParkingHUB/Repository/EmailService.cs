using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using ParkingHUB.DTO;
using ParkingHUB.Interface;

namespace ParkingHUB.Repository
{
    public class EmailService : IEmail
    {
        public void SendEmail(EmailDTO request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(request.From));
            email.To.Add(MailboxAddress.Parse("alden.lakin@ethereal.email"));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("alden.lakin@ethereal.email", "KHRuhH6bxpQYEyhkGY");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
