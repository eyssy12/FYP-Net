namespace CMS.Dashboard.Test.Services
{
    using System.Net;
    using System.Net.Mail;
    using Models;

    public class EmailSenderService : INotify<EmailMessage>
    {
        private readonly string YahooSmtp = "smtp.mail.yahoo.com";
        private readonly int YahooPort = 587;

        private readonly string ServiceEmail = "fypcms@yahoo.co.uk",
            Credentials = "Final_Year_Project";

        public void SendNotification(EmailMessage message)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(message.From);
                mail.To.Add(message.To);
                mail.Subject = message.Subject;
                mail.Body = message.Body;

                using (SmtpClient smtp = new SmtpClient(this.YahooSmtp, this.YahooPort))
                {
                    smtp.Credentials = new NetworkCredential(message.From, message.Password);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
    }
}