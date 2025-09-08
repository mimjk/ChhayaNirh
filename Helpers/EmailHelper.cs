using System.Net;
using System.Net.Mail;

namespace ChhayaNirh.Helpers
{
    public static class EmailHelper
    {
        // Gmail App Password ব্যবহার করে email পাঠানো
        public static void SendEmail(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress("afiasiddiquemim@gmail.com", "ChhayaNirh"); // তোমার Gmail
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "ljtd ltdt telb vvur"; // 16-character App Password

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}
