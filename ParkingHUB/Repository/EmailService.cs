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
            email.To.Add(MailboxAddress.Parse("obie40@ethereal.email"));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("obie40@ethereal.email", "pDjeFQEGqTgmF24Dya");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
