//using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
//using System.Net;
//using System.Net.Mail;

//namespace RMSProjectAPI
//{
//    public static class MailService
//    {
//        public static void SendEmail (string email, string subject, string body)
//        {

//            var client = new SmtpClient("live.smtp.mailtrap.io", 587)

//            {

//                Credentials = new NetworkCredential("smtp@mailtrap.io", "1ee70520331f6c7a79f05ebab0f98295"),

//                EnableSsl = true

//            };

//            client.Send("contact@abdelrahmankasem.com", email, subject, body);

//            System.Console.WriteLine("Sent");
//        }
//    }
//}

using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Net;
using System.Net.Mail;

namespace RMSProjectAPI
{
    public static class MailService
    {
        public static void SendEmail(string email, string subject, string body)
        {
            var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("e3f6a18cfabbfe", "30c45f2f1ee2d4"),
                EnableSsl = true
            };
            client.Send("from@example.com", email, subject, body);
            System.Console.WriteLine("Sent");
        }
    }
}